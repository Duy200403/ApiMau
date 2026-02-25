using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApiWebsite.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BC = BCrypt.Net.BCrypt;
using System.Linq;
using ApiWebsite.Helper;

namespace ApiWebsite.Core.Services
{
    public class SeedDataService
    {
        public static async Task SeedAccount(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IAccountService>();
            string userName = "admin";
            string pass = "Admin$$1234";
            string salt = BC.GenerateSalt();
            Account acc = new Account()
            {
                FullName = userName,
                Username = userName,
                Roles = nameof(Helper.Role.admin),
                PasswordHash = BC.HashPassword(pass),
                Email = "admin@tnh99.com.vn",
                AccessFailedCount = 0,
                Pseudonym = "admin_tnh99",
                PhoneNumber = "0979666666",
                IsLock = false,
                CreatedDate = DateTime.UtcNow,
                Salt = salt,
                IsActive = true,
            };
            // 
            var checkExits = await svc.AnyAsync(x => x.Username == userName);
            if (!checkExits)
            {
                await svc.AddOneAsync(acc);
            }

            userName = "manager";
            pass = "Admin$$1234";
            salt = BC.GenerateSalt();
            acc = new Account()
            {
                FullName = userName,
                Username = userName,
                Roles = nameof(Helper.Role.manager),
                PasswordHash = BC.HashPassword(pass),
                Email = "manager@tnh99.com.vn",
                AccessFailedCount = 0,
                Pseudonym = "manager_tnh99",
                PhoneNumber = "0979888888",
                IsLock = false,
                CreatedDate = DateTime.UtcNow,
                Salt = salt,
                IsActive = true,
            };
            // 
            checkExits = await svc.AnyAsync(x => x.Username == userName);
            if (!checkExits)
            {
                await svc.AddOneAsync(acc);
            }

            userName = "editor";
            pass = "Admin$$1234";
            salt = BC.GenerateSalt();
            acc = new Account()
            {
                FullName = userName,
                Username = userName,
                Roles = nameof(Helper.Role.editor),
                PasswordHash = BC.HashPassword(pass),
                Email = "editor@gmail.com",
                AccessFailedCount = 0,
                Pseudonym = "editor_108hospital",
                PhoneNumber = "0979888888",
                IsLock = false,
                CreatedDate = DateTime.UtcNow,
                Salt = salt,
                IsActive = true,
            };
            // 
            checkExits = await svc.AnyAsync(x => x.Username == userName);
            if (!checkExits)
            {
                await svc.AddOneAsync(acc);
            }

            // var svcNoiDungDanhSach = scope.ServiceProvider.GetRequiredService<INoiDungDeNghiDanhSachService>();

            // var ckeckNoiDungDanhSach = await svcNoiDungDanhSach.UpdateManyNoiDungDanhSach();

            // if (ckeckNoiDungDanhSach == true) {
            //     return;
            // }


            // var svcDuLieuOracle = scope.ServiceProvider.GetRequiredService<ISoLuongCancelService>();

            // var startDate = new DateTime(2021, 1, 1, 0, 0, 0);
            // var endDate = new DateTime(2024, 10, 14, 23, 59, 59);
            // var resultList = await svcDuLieuOracle.GetHmsFeeCount(startDate, endDate);

            // // Console.WriteLine("listSoLuongCancel", resultList);

            // if (resultList.Count > 0)
            // {
            //     await svcDuLieuOracle.AddManyAsync(resultList);
            // }

            // var startDate = new DateTime(2021, 1, 1, 0, 0, 0);
            // var endDate = new DateTime(2024, 12, 26, 23, 59, 59);
            // var resultList = await svcDuLieuOracle.GetHmsFeeCountStatusR(startDate, endDate);

            // Console.WriteLine("listSoLuongCancel", resultList);

            // if (resultList.Count > 0)
            // {
            //     await svcDuLieuOracle.AddManyAsync(resultList);
            // }
            // var svcDuLieu = scope.ServiceProvider.GetRequiredService<IGiamSatChinhSuaDuLieuBenhVienService>();

            // var checkItem = await svcDuLieu.UpdateManyGiamSatChinhSuaDuLieuBenhVien();

            // var svcDVDN = scope.ServiceProvider.GetRequiredService<IDonViDeNghiService>();

            // var request = new DonViDeNghiPagingFilter();

            // request.PageIndex = 1;
            // request.PageSize = 1000;
            // var lstDonViDeNghi = await svcDVDN.GetAllPaging(request);
            // foreach(var item in lstDonViDeNghi.Data) {
            //     var newItem = new DonViDeNghi();
            //     newItem.ID = item.ID;
            //     newItem.IDOriginal = item.ID;
            //     newItem.TenTat = item.TenTat;
            //     newItem.Ten = item.Ten;
            //     newItem.MasterID = item.MasterID;
            //     newItem.DiaChi = item.DiaChi;
            //     newItem.Khoi = item.Khoi;
            //     newItem.IsDeletedOriginal = item.IsDeletedOriginal;
            //     newItem.IsLockedOriginal = item.IsLockedOriginal;
            //     newItem.CreatedDateOriginal = item.CreatedDateOriginal;
            //     newItem.CreatedByOriginal = item.CreatedByOriginal;
            //     newItem.UpdatedDateOriginal = item.UpdatedDateOriginal;
            //     newItem.UpdateByOriginal = item.UpdateByOriginal;
            //     newItem.Tree = item.Tree;
            //     newItem.STT = item.STT;

            //     var checkItem = await svcDVDN.UpsertAsync(newItem);
            // }

            // return;

            // dynamic lstOriginalAccount = await svc.GetListOriginalAccount();
            // IEnumerable<dynamic> lstOriginalAccountEnumerable = lstOriginalAccount as IEnumerable<dynamic>;
            // List<string> userNamesOriginal = lstOriginalAccountEnumerable.Select(model => (string)model.UserName.Value).ToList();
            // var lstAccount = new List<Account>();

            // var predicateFilterAccount = PredicateBuilder.True<Account>(); // khởi tạo mệnh đề truy vấn linq
            // predicateFilterAccount = predicateFilterAccount.And(x => true);
            // predicateFilterAccount = predicateFilterAccount.And(x => userNamesOriginal.Contains(x.Username));

            // var existingUsernames = new HashSet<string>((await svc.GetPaginatedAsync(predicateFilterAccount, 1, 10000)).Select(x => x.Username));

            // var count = 0;

            // foreach (var model in lstOriginalAccount)
            // {
            //     // count++;
            //     string userNameModel = model.UserName.Value;
            //     // var existAccount = await svc.GetOneAsync(x => x.Username == userNameModel);
            //     if (!existingUsernames.Contains(userNameModel))
            //     {
            //         Account account = new Account
            //         {
            //             Id = Guid.NewGuid(),
            //             Username = userNameModel,
            //             PasswordHash = BC.HashPassword("Admin$$1234"), // = BC.HashPassword(model.Password, salt)
            //             Salt = BC.GenerateSalt(),
            //             IsActive = true,
            //             CreatedDate = DateTime.UtcNow
            //         };

            //         lstAccount.Add(account);
            //     }

            //     // if (count > 9)
            //     // {
            //     //     break;
            //     // }
            // }

            // if (lstAccount.Count > 0)
            // {
            //     await svc.AddManyAsync(lstAccount);
            // }
        }
    }
}
