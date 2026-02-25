using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApiWebsite.Model;
using Microsoft.Extensions.Options;

namespace ApiWebsite.Helper
{
    public static class FilePathHelper
    {
        public static (string, string, string) GetDirectoryPath(IOptions<List<VirtualPathConfig>> configuration, string ext)
        {
            string alias = _mappings.ContainsKey(ext) ? _mappings[ext] : Enum.GetName(typeof(FileAliAs), FileAliAs.document);
            var f = configuration.Value.FirstOrDefault(x => x.Alias == alias);
            if (!Directory.Exists(f.RealPath))
            {
                try
                {
                    Directory.CreateDirectory(f.RealPath);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
            return (f.RealPath, f.RequestPath, alias);
        }

        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
            {".gif", Enum.GetName(typeof(FileAliAs), FileAliAs.images)},
            {".jpe", Enum.GetName(typeof(FileAliAs), FileAliAs.images)},
            {".jpeg", Enum.GetName(typeof(FileAliAs), FileAliAs.images)},
            {".jpg", Enum.GetName(typeof(FileAliAs), FileAliAs.images)},
            {".png", Enum.GetName(typeof(FileAliAs), FileAliAs.images)},

            {".doc", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},
            {".docx", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},
            {".txt", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},
            {".xls", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},
            {".xlsx", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},
            {".ppt", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},
            {".pptx", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},

            {".zip", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},
            {".rar", Enum.GetName(typeof(FileAliAs), FileAliAs.document)},

            {".mp4", Enum.GetName(typeof(FileAliAs), FileAliAs.videos)},
            {".m4v", Enum.GetName(typeof(FileAliAs), FileAliAs.videos)},
            // {".txt", Enum.GetName(typeof(FileAliAs), FileAliAs.wellknown)},
        };
    }
}