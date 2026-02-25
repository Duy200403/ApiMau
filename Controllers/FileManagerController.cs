using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ApiWebsite.Common;
using ApiWebsite.Core.Services;
using ApiWebsite.Helper;
using ApiWebsite.Helper.Middleware;
using ApiWebsite.Model;
using ApiWebsite.Models;
using AutoMapper;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiWebsite.Controllers
{
    public class FileManagerController : BaseController
    {
        private readonly ILogger<FileManagerController> _logger;
        private readonly IFileManagerService _ifileManagerService;
        private readonly IOptions<List<VirtualPathConfig>> _iVirtualPathConfig;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        public FileManagerController(ILogger<FileManagerController> logger, IFileManagerService ifileManagerService, IOptions<List<VirtualPathConfig>> IVirtualPathConfig, ILogService logService, IMapper mapper)
        {
            _logger = logger;
            _ifileManagerService = ifileManagerService;
            _iVirtualPathConfig = IVirtualPathConfig;
            _logService = logService;
            _mapper = mapper;
        }

        [Authorize(Role.admin, Role.manager, Role.publisher, Role.editor)]
        [HttpGet("[action]")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(PagedResult<FileManagerRespone>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllPaging([FromQuery] FileManagerPagingFilter request)
        {
            var result = await _ifileManagerService.GetAllPaging(request);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetDirectoryPath()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            return Ok(path);
        }

        [Authorize(Role.admin, Role.manager, Role.publisher, Role.editor)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload()
        {
            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
            {
                List<FileManager> files = new List<FileManager>();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                        var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();

                        (string directory, string virtualPath, string fileType) = FilePathHelper.GetDirectoryPath(_iVirtualPathConfig, ext);
                        var fullName = fileName + "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                        string thumbName = fullName + "_thumb" + ext;
                        fullName += ext;

                        var filePath = Path.Combine(directory, fullName);
                        var thumbPath = Path.Combine(directory, thumbName);

                        if (fileType == Enum.GetName(typeof(FileAliAs), FileAliAs.images))
                        {
                            using (var input = formFile.OpenReadStream())
                            {
                                using (var output = System.IO.File.Create(filePath))
                                {
                                    using (var image = new MagickImage(input))
                                    {
                                        if (image.Width > 1920 || image.Height > 1080)
                                        {
                                            image.Resize(1920, 1080);
                                            image.Write(output);
                                        }
                                        else
                                        {
                                            input.Position = 0;
                                            await input.CopyToAsync(output);
                                        }
                                    }

                                    var optimizer = new ImageOptimizer
                                    {
                                        IgnoreUnsupportedFormats = true,
                                    };

                                    output.Position = 0;
                                    optimizer.Compress(output);

                                    // using (var fileStream = System.IO.File.OpenWrite(filePath))
                                    // {
                                    //     await output.CopyToAsync(fileStream);
                                    // }
                                }
                            }

                        }
                        else
                        {
                            // upload file gốc vào server
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }

                        

                        if (fileType == Enum.GetName(typeof(FileAliAs), FileAliAs.images))
                        {
                            // tạo thumb ảnh
                            Image image = Image.FromFile(filePath);
                            Size thumbnailSize = GetThumbnailSize(image);
                            using (Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero))
                            {
                                thumbnail.Save(thumbPath);
                            }
                        }
                        else
                        {
                            thumbPath = "";
                        }

                        var fileManager = new FileManager
                        {
                            Id = Guid.NewGuid(),
                            PhysicalPath = filePath,
                            PhysicalThumbPath = thumbPath,
                            FilePath = Path.Combine(virtualPath, fullName).Replace("\\", "/"),
                            ThumbPath = string.IsNullOrEmpty(thumbPath) ? "" : Path.Combine(virtualPath, thumbName).Replace("\\", "/"),
                            FileType = formFile.ContentType,
                            Name = fileName,
                            FileSizeInKB = 0
                        };
                        await _ifileManagerService.AddOneAsync(fileManager);
                        files.Add(fileManager);
                    }
                }
                var paramTrace = Newtonsoft.Json.JsonConvert.SerializeObject(files);
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "FileManagerController, Upload, Ok", paramTrace);
                // return Ok(_mapper.Map<IEnumerable<FileManagers>>(files));
                return Ok(files);
            }
            await _logService.AddLogWebInfo(LogLevelWebInfo.error, "FileManagerController, Upload, BadRequest", null);
            return BadRequest();
        }

        [Authorize(Role.admin, Role.manager, Role.publisher, Role.editor)]
        [HttpGet("{filename}")]
        [ProducesResponseType(typeof(PagedResult<FileManager>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFile(string filename)
        {
          var file = await _ifileManagerService.GetOneAsync(x => x.Name == filename);
          if (file != null)
          {
            // var result = new System.IO.FileInfo(file.FilePath);
            var result = new System.IO.FileInfo(file.PhysicalPath);
            if (result.Exists)
            {
              return Ok(result.OpenRead());
            }
          }
          await _logService.AddLogWebInfo(LogLevelWebInfo.error, "FileManagerController, Get, BadRequest", filename);
          return BadRequest();
        }

        [Authorize(Role.admin, Role.manager, Role.publisher, Role.editor)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var file = await _ifileManagerService.GetByIdAsync(id);
            if (file != null)
            {
                var result = await _ifileManagerService.DeleteAsync(id);
                string path = System.IO.Directory.GetCurrentDirectory();
                //   xóa trong ổ cứng
                System.IO.File.Delete(path + "\\" + file.PhysicalPath);
                if (file.PhysicalThumbPath != "")
                {
                    System.IO.File.Delete(path + "\\" + file.PhysicalThumbPath);
                }
                await _logService.AddLogWebInfo(LogLevelWebInfo.trace, "FileManagerController, Delete, Ok", id.ToString());
                return Ok();
            }
            return BadRequest();
        }
        private Size GetThumbnailSize(Image original)
        {
            // Maximum size of any dimension.
            const int maxPixels = 200;
            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;
            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)maxPixels / originalWidth;
            }
            else
            {
                factor = (double)maxPixels / originalHeight;
            }
            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }
    }
}