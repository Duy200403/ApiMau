using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ApiWebsite.Core.Base;
using Microsoft.EntityFrameworkCore;
using ApiWebsite.Helper;
using System.Net.Http;
using System.Net.Http.Headers;
using ApiWebsite.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApiWebsite.Core.Services
{
    public class CronJobUpdateDonViDeNghiService : CronJobService
    {
        private readonly ILogger<CronJobUpdateDonViDeNghiService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private static bool isProcessed = false;
        private DateTime firstTimeStart;
        private DateTime firstTimeEnd;
        // private DateTime secondTimeStart;
        // private DateTime secondTimeEnd;
        // private readonly ApplicationDbContext _dbContext;

        public CronJobUpdateDonViDeNghiService(IServiceProvider serviceProvider, IScheduleConfig<CronJobUpdateDonViDeNghiService> config, ILogger<CronJobUpdateDonViDeNghiService> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            // _dbContext = dbContext;
            _logger = logger;
            _serviceProvider = serviceProvider;

            isProcessed = false;
            var date = DateTime.Now;

            firstTimeStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 30, 0);
            firstTimeEnd = firstTimeStart.AddMinutes(29);

            // secondTimeStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 12, 30, 0);
            // secondTimeEnd = secondTimeStart.AddMinutes(29);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Send Email starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var date = DateTime.Now;
            // TODO noinh test
            // _logger.LogInformation($"Cronjob runing on: {date:hh:mm:ss}");

            // if (((date.Hour == firstTimeStart.Hour && date.Minute >= firstTimeStart.Minute && date.Minute <= firstTimeEnd.Minute)
            //     || (date.Hour == secondTimeStart.Hour && date.Minute >= secondTimeStart.Minute && date.Minute <= secondTimeEnd.Minute)) && date.DayOfWeek != DayOfWeek.Sunday)
            // {
            //     // TODO noinh test
            //     // _logger.LogInformation($"Cronjob with hour success: {date:hh:mm:ss}");

            //     if (isProcessed == false)
            //     {
            //         // TODO noinh test
            //         // _logger.LogInformation($"Cronjob with isProcessed = false then run Task: {date:hh:mm:ss}");

            //         using var scope = _serviceProvider.CreateScope();
            //         ISendEmailService _iSendEmailService = scope.ServiceProvider.GetRequiredService<ISendEmailService>();
            //         IGroupCusService _iGroupService = scope.ServiceProvider.GetRequiredService<IGroupCusService>();
            //         var lstGroup = _iGroupService.GetAll<GroupCus>(x => x.IsActive && x.ExprieDate >= DateTime.UtcNow).ToList();
            //         _iSendEmailService.sendEmail(lstGroup);

            //         // đánh dấu đã xử lý xong
            //         isProcessed = true;
            //     }
            // }
            // else
            // {
            //     // đặt lại trạng thái chưa xử lý
            //     isProcessed = false;
            // }

            // if (date.Hour == firstTimeStart.Hour && date.Minute >= firstTimeStart.Minute && date.Minute <= firstTimeEnd.Minute)
            // // if (true)
            // {
            //     // TODO noinh test
            //     // _logger.LogInformation($"Cronjob with hour success: {date:hh:mm:ss}");

            //     var listSoLuongBaoCao = new List<SoLuongBaoCao>();
            //     var listSoLuongCancel = new List<SoLuongCancel>();

            //     if (isProcessed == false)
            //     {
            //         // TODO noinh test
            //         // _logger.LogInformation($"Cronjob with isProcessed = false then run Task: {date:hh:mm:ss}");

            //         IAccountHisService _iAccountHisService = scope.ServiceProvider.GetRequiredService<IAccountHisService>();

            //         var lstAccounts = await _iAccountHisService.GetAllPaging(new AccountHisPagingFilter(){
            //             PageSize = 1000,
            //             PageIndex = 1
            //         });

            //         // ISendEmailService _iSendEmailService = scope.ServiceProvider.GetRequiredService<ISendEmailService>();
            //         ISoLuongBaoCaoService svcSoLuongBaoCaoService = scope.ServiceProvider.GetRequiredService<ISoLuongBaoCaoService>();

            //         foreach (var account in lstAccounts.Data)
            //         {
            //             // var insertCount = await _iSoLuongBaoCaoService.GetAuditTrailCount(username, "INSERT");
            //             var updateCount = await svcSoLuongBaoCaoService.GetAuditTrailCount(account.Username, "UPDATE");
            //             var deleteCount = await svcSoLuongBaoCaoService.GetAuditTrailCount(account.Username, "DELETE");
            //             // var selectCount = await _iSoLuongBaoCaoService.GetAuditTrailCount(username, "SELECT");
            //             // var hmsFeeCount = await _iSoLuongBaoCaoService.GetHmsFeeCount(account.Username,
            //             //                     new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0),
            //             //                     new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59));

            //             listSoLuongBaoCao.Add(new SoLuongBaoCao(){
            //                 Username = account.Username,
            //                 AccountHisId = account.Id,
            //                 SoLuongUpdate = updateCount,
            //                 SoLuongDelete = deleteCount,
            //                 ThoiGianBaoCaoBatDau = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0),
            //                 ThoiGianBaoCaoKetThuc = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59)
            //             });
            //         }

            //         if (listSoLuongBaoCao.Count > 0)
            //         {
            //             await svcSoLuongBaoCaoService.AddManyAsync(listSoLuongBaoCao);
            //         }

            //         var svcSoLuongCancelService = scope.ServiceProvider.GetRequiredService<ISoLuongCancelService>();

            //         var startDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            //         var endDay = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

            //         var result = await svcSoLuongCancelService.GetHmsFeeCountToday(startDay, endDay);

            //         var soLuongCancel = new SoLuongCancel(){
            //                 SoLuongBaoCaoCancel = result.TotalCount,
            //                 TongTienBaoCaoCancel = result.TotalMoney,
            //                 Status = "C",
            //                 ThoiGianBaoCaoBatDau = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0),
            //                 ThoiGianBaoCaoKetThuc = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59)
            //             };

            //         if (soLuongCancel != null)
            //         {
            //             await svcSoLuongCancelService.AddOneAsync(soLuongCancel);
            //         }

            //         var resultStatusR = await svcSoLuongCancelService.GetHmsFeeCountStatusRToday(startDay, endDay);

            //         var soLuongCancelStatusR = new SoLuongCancel(){
            //                 SoLuongBaoCaoCancel = result.TotalCount,
            //                 TongTienBaoCaoCancel = result.TotalMoney,
            //                 Status = "R",
            //                 ThoiGianBaoCaoBatDau = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0),
            //                 ThoiGianBaoCaoKetThuc = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59)
            //             };

            //         if (soLuongCancelStatusR != null)
            //         {
            //             await svcSoLuongCancelService.AddOneAsync(soLuongCancelStatusR);
            //         }

            //         // var lstGroup = _iGroupService.GetAll<GroupCus>(x => x.IsActive && x.ExprieDate >= DateTime.UtcNow).ToList();
            //         // _iSendEmailService.sendEmail(lstGroup);

            //         // đánh dấu đã xử lý xong
            //         isProcessed = true;
            //     }
            // }
            // else
            // {
            //     // đặt lại trạng thái chưa xử lý
            //     isProcessed = false;
            // }

            // var svc = scope.ServiceProvider.GetRequiredService<IDonViDeNghiService>();
            // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // // var existingDonViDeNghi = await dbContext.DonViDeNghi.ToListAsync();
            // // // var existingDonViDeNghi = new List<DonViDeNghi>();
            // // var existingDonViDeNghiID = new HashSet<string>(existingDonViDeNghi.Select(x => x.IDOriginal));
            // var listDonViDeNghi = new List<DonViDeNghi>();

            // var timeNow = DateTime.Now;

            // using (HttpClient client = new HttpClient())
            // {
            //     // Specify the URI to get data from
            //     string uri = "http://108.108.108.52:10808/api/DM_DonVi/SelectAll";

            //     try
            //     {
            //         // Send a GET request to the specified Uri
            //         HttpResponseMessage response = await client.GetAsync(uri);
            //         response.EnsureSuccessStatusCode();
            //         string responseBody = await response.Content.ReadAsStringAsync();
            //         var jsonResult = JsonConvert.DeserializeObject(responseBody).ToString();
            //         dynamic result = JsonConvert.DeserializeObject(jsonResult);
            //         dynamic lstDonViDeNghi = result._Data;
            //         // dynamic lstDonViDeNghi = result._Data;

            //         foreach (var model in lstDonViDeNghi)
            //         {
            //             // count++;
            //             string tenModel = model.Ten.Value;
            //             string tenTatModel = model.TenTat.Value;
            //             long idOriginalModel = model.ID.Value;
            //             var existDonViDeNghi = await svc.GetOneAsync(x => x.IDOriginal == idOriginalModel);
            //             if (existDonViDeNghi == null)
            //             {
            //                 DonViDeNghi donViDeNghi = new DonViDeNghi
            //                 {
            //                     Id = Guid.NewGuid(),
            //                     IDOriginal = model.ID.Value,
            //                     TenTat = model.TenTat.Value,
            //                     Ten = model.Ten.Value,
            //                     MasterID = model.MasterID.Value,
            //                     DiaChi = model.DiaChi.Value,
            //                     Khoi = model.Khoi.Value,
            //                     IsDeletedOriginal = model.IsDeleted.Value,
            //                     IsLockedOriginal = model.IsLocked.Value,
            //                     CreatedDateOriginal = model.CreatedDate.Value,
            //                     CreatedByOriginal = model.CreatedBy.Value,
            //                     UpdatedDateOriginal = model.UpdatedDate.Value,
            //                     UpdateByOriginal = model.UpdateBy.Value,
            //                     Tree = model.Tree.Value,
            //                     STT = model.STT.Value,
            //                 };

            //                 listDonViDeNghi.Add(donViDeNghi);
            //             }

            //             // var pagedResult = new PagedResult<DonViDeNghiResponse>()
            //             // {
            //             //     TotalRecords = 0,
            //             //     PageSize = 0,
            //             //     PageIndex = 0,
            //             //     Data = lstDonViDeNghi
            //             // };

            //             // var parsedData = JsonSerializer.Deserialize<DonViDeNghi>(parsedJsonModel._Data);

            //             // Print the response body to the console
            //             // return Ok(lstDonViDeNghi);
            //         }

            //         if (listDonViDeNghi.Count > 0)
            //         {
            //             await svc.AddManyAsync(listDonViDeNghi);
            //         }
            //     }
            //     catch (HttpRequestException e)
            //     {
            //         // Handle any errors that occurred during the request
            //         Console.WriteLine("\nException Caught!");
            //         Console.WriteLine("Message :{0} ", e.Message);
            //     }
            // }

            // var lstBookDeviceNotFinish = await dbContext.BookDevice.Include(x => x.Device).Where(x => x.EndBookTime < timeNow && x.BookStatus == BookStatus.approvedAndBooked).ToListAsync();

            // // var lstBookDeviceNotFinish = lstBookDeviceNotFinishData.Data;

            // foreach (var item in lstBookDeviceNotFinish)
            // {
            //     item.BookStatus = BookStatus.finishBooking;
            //     item.Device.CurrentNumberOfUse++;
            // }

            // if (lstBookDeviceNotFinish.Count > 0)
            // {
            //     await dbContext.SaveChangesAsync();
            // }


            // return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Send Email is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
