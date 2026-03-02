using ApiWebsite.Core.Base;
using ApiWebsite.Helper;
using ApiWebsite.Models;
using ApiWebsite.Models.System.DonViDeNghi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

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
        }
            public static async Task SeedBiddingData(IServiceProvider serviceProvider)
            {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Chỉ chạy Seed nếu DB chưa có ProcessBranch nào
            if (context.ProcessBranches.Any()) return;

            #region 1. SEED ĐƠN VỊ ĐỀ NGHỊ
            var lstDonVi = new List<DonViDeNghi>
            {
                new DonViDeNghi { Code = "DVMS", Name = "Đơn vị mua sắm", CreatedDate = DateTime.Now },
                new DonViDeNghi { Code = "PKHTH", Name = "Phòng Kế hoạch tổng hợp", CreatedDate = DateTime.Now },
                new DonViDeNghi { Code = "PTC", Name = "Phòng Tài chính", CreatedDate = DateTime.Now },
                new DonViDeNghi { Code = "BQLDT", Name = "Ban Quản lý đấu thầu", CreatedDate = DateTime.Now },
                new DonViDeNghi { Code = "GĐ", Name = "Giám đốc", CreatedDate = DateTime.Now },
                new DonViDeNghi { Code = "TMHC", Name = "Tham mưu hành chính", CreatedDate = DateTime.Now }
            };
            context.DonViDeNghi.AddRange(lstDonVi);
            await context.SaveChangesAsync();

            long idDVMS = lstDonVi.First(x => x.Code == "DVMS").Id;
            long idPTC = lstDonVi.First(x => x.Code == "PTC").Id;
            long idBQLDT = lstDonVi.First(x => x.Code == "BQLDT").Id;
            long idGD = lstDonVi.First(x => x.Code == "GĐ").Id;
            long idTMHC = lstDonVi.First(x => x.Code == "TMHC").Id;
            #endregion

            #region 2. SEED NHÁNH QUY TRÌNH
            var branchKhoiTao = new ProcessBranch { Id = Guid.NewGuid(), BranchCode = "QT-KHOITAO", BranchName = "Quy trình Khởi tạo chung", Description = "Từ Đề xuất đến phê duyệt KHLCNT", CreatedDate = DateTime.Now };
            var branchNhanh1 = new ProcessBranch { Id = Guid.NewGuid(), BranchCode = "QT-223", BranchName = "Nhánh 1: KHLCNT Chỉ định thầu rút gọn", Description = "Áp dụng cho gói thầu quy mô nhỏ, khẩn cấp", CreatedDate = DateTime.Now };
            var branchNhanh2 = new ProcessBranch { Id = Guid.NewGuid(), BranchCode = "QT-222", BranchName = "Nhánh 2: Đấu thầu rộng rãi", Description = "Qua mạng, 1 giai đoạn 1 túi hồ sơ", CreatedDate = DateTime.Now };

            context.ProcessBranches.AddRange(branchKhoiTao, branchNhanh1, branchNhanh2);
            await context.SaveChangesAsync();
            #endregion

            #region 3. SEED BƯỚC THỰC HIỆN & FORM ĐỘNG
            var lstSteps = new List<ProcessStep>();
            var lstAttrs = new List<StepAttribute>();

            // --- NHÁNH KHỞI TẠO ---
            var kt_b1 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchKhoiTao.Id, StepCode = "KT-B1", StepName = "Lập đề xuất mua sắm", Order = 1, TargetDonViId = idDVMS, SlaDays = 2, RequiresAttachment = true };
            var kt_b2 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchKhoiTao.Id, StepCode = "KT-B2", StepName = "Phòng KHTH Duyệt", Order = 2, TargetDonViId = lstDonVi.First(x => x.Code == "PKHTH").Id, SlaDays = 2, RequiresAttachment = false };
            var kt_b3 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchKhoiTao.Id, StepCode = "KT-B3", StepName = "Phòng Tài chính Duyệt", Order = 3, TargetDonViId = idPTC, SlaDays = 2, RequiresAttachment = false };
            var kt_b4 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchKhoiTao.Id, StepCode = "KT-B4", StepName = "Ban QLĐT Duyệt", Order = 4, TargetDonViId = idBQLDT, SlaDays = 2, RequiresAttachment = true };
            var kt_b5 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchKhoiTao.Id, StepCode = "KT-B5", StepName = "Giám đốc phê duyệt", Order = 5, TargetDonViId = idGD, SlaDays = 1, RequiresAttachment = true };
            lstSteps.AddRange(new[] { kt_b1, kt_b2, kt_b3, kt_b4, kt_b5 });

            lstAttrs.AddRange(new[] {
                new StepAttribute { Id = Guid.NewGuid(), ProcessStepId = kt_b1.Id, AttributeCode = "ten_de_xuat", AttributeName = "Tên đề xuất", InputType = 1, IsRequired = true, DisplayOrder = 1 },
                new StepAttribute { Id = Guid.NewGuid(), ProcessStepId = kt_b1.Id, AttributeCode = "gia_tri_du_kien", AttributeName = "Giá trị dự kiến (VNĐ)", InputType = 1, IsRequired = true, DisplayOrder = 2 },
                new StepAttribute { Id = Guid.NewGuid(), ProcessStepId = kt_b5.Id, AttributeCode = "ngay_phe_duyet", AttributeName = "Ngày phê duyệt", InputType = 3, IsRequired = true, DisplayOrder = 1 }
            });

            // --- NHÁNH 1: CHỈ ĐỊNH THẦU ---
            var n1_b1 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchNhanh1.Id, StepCode = "N1-B1", StepName = "Soạn và gửi dự thảo Hợp đồng", Order = 1, TargetDonViId = idDVMS, SlaDays = 2, RequiresAttachment = true };
            var n1_b2 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchNhanh1.Id, StepCode = "N1-B2", StepName = "Tham mưu HC thẩm định dự thảo", Order = 2, TargetDonViId = idTMHC, SlaDays = 1, RequiresAttachment = true };
            var n1_b3 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchNhanh1.Id, StepCode = "N1-B3", StepName = "Tờ trình phê duyệt KQLCNT", Order = 3, TargetDonViId = idDVMS, SlaDays = 3, RequiresAttachment = true };
            var n1_b4 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchNhanh1.Id, StepCode = "N1-B4", StepName = "Phê duyệt KQLCNT", Order = 4, TargetDonViId = idGD, SlaDays = 1, RequiresAttachment = true };
            lstSteps.AddRange(new[] { n1_b1, n1_b2, n1_b3, n1_b4 });

            lstAttrs.AddRange(new[] {
                new StepAttribute { Id = Guid.NewGuid(), ProcessStepId = n1_b3.Id, AttributeCode = "ten_nha_thau", AttributeName = "Tên nhà thầu trúng", InputType = 1, IsRequired = true, DisplayOrder = 1 },
                new StepAttribute { Id = Guid.NewGuid(), ProcessStepId = n1_b3.Id, AttributeCode = "gia_trung_thau", AttributeName = "Giá trúng thầu", InputType = 1, IsRequired = true, DisplayOrder = 2 }
            });

            // --- NHÁNH 2: ĐẤU THẦU RỘNG RÃI ---
            var n2_b1 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchNhanh2.Id, StepCode = "N2-B1", StepName = "Lập tờ trình E-HSMT", Order = 1, TargetDonViId = idDVMS, SlaDays = 2, RequiresAttachment = true };
            var n2_b2 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchNhanh2.Id, StepCode = "N2-B2", StepName = "Thẩm định E-HSMT", Order = 2, TargetDonViId = idDVMS, SlaDays = 3, RequiresAttachment = true };
            var n2_b3 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchNhanh2.Id, StepCode = "N2-B3", StepName = "Ban QLĐT Duyệt", Order = 3, TargetDonViId = idBQLDT, SlaDays = 3, RequiresAttachment = true };
            var n2_b4 = new ProcessStep { Id = Guid.NewGuid(), ProcessBranchId = branchNhanh2.Id, StepCode = "N2-B4", StepName = "Phê duyệt E-HSMT", Order = 4, TargetDonViId = idGD, SlaDays = 3, RequiresAttachment = true };
            lstSteps.AddRange(new[] { n2_b1, n2_b2, n2_b3, n2_b4 });

            context.ProcessSteps.AddRange(lstSteps);
            context.StepAttributes.AddRange(lstAttrs);
            await context.SaveChangesAsync();
            #endregion

            #region 4. SEED GÓI THẦU (MỖI NHÁNH 2 CASE)

            // ================== NHÁNH KHỞI TẠO ==================
            // Case 1: Mượt mà (Hoàn thành 100%)
            var p1 = new BiddingProject { Id = Guid.NewGuid(), ProjectCode = "KT-2026-001", ProjectName = "Mua sắm máy X-Quang", ProcessBranchId = branchKhoiTao.Id, CurrentStepOrder = 5, Status = "Completed", CreatedDate = DateTime.Now.AddDays(-10) };
            context.BiddingProjects.Add(p1);
            context.ProjectSteps.AddRange(
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p1.Id, ProcessStepId = kt_b1.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-10), CompletedDate = DateTime.Now.AddDays(-9), FormDataJson = "{\"ten_de_xuat\":\"Đề xuất mua máy X-Quang\",\"gia_tri_du_kien\":\"500000000\"}" },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p1.Id, ProcessStepId = kt_b2.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-9), CompletedDate = DateTime.Now.AddDays(-8) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p1.Id, ProcessStepId = kt_b3.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-8), CompletedDate = DateTime.Now.AddDays(-7) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p1.Id, ProcessStepId = kt_b4.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-7), CompletedDate = DateTime.Now.AddDays(-6) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p1.Id, ProcessStepId = kt_b5.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-6), CompletedDate = DateTime.Now.AddDays(-5), FormDataJson = "{\"ngay_phe_duyet\":\"2026-02-22\"}" }
            );

            // Case 2: Đang kẹt ở Bước 3 (Phòng Tài chính duyệt)
            var p2 = new BiddingProject { Id = Guid.NewGuid(), ProjectCode = "KT-2026-002", ProjectName = "Bảo trì thang máy", ProcessBranchId = branchKhoiTao.Id, CurrentStepOrder = 3, Status = "InProgress", CreatedDate = DateTime.Now.AddDays(-5) };
            context.BiddingProjects.Add(p2);
            context.ProjectSteps.AddRange(
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p2.Id, ProcessStepId = kt_b1.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-5), CompletedDate = DateTime.Now.AddDays(-4), FormDataJson = "{\"ten_de_xuat\":\"Bảo trì thang máy\",\"gia_tri_du_kien\":\"150000000\"}" },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p2.Id, ProcessStepId = kt_b2.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-4), CompletedDate = DateTime.Now.AddDays(-3) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p2.Id, ProcessStepId = kt_b3.Id, Status = "Processing", StartDate = DateTime.Now.AddDays(-3), Comments = "Chưa có nguồn kinh phí phân bổ. Yêu cầu xem lại" }, // Kẹt ở đây
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p2.Id, ProcessStepId = kt_b4.Id, Status = "Pending" },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p2.Id, ProcessStepId = kt_b5.Id, Status = "Pending" }
            );


            // ================== NHÁNH 1: CHỈ ĐỊNH THẦU ==================
            // Case 3: Mượt mà
            var p3 = new BiddingProject { Id = Guid.NewGuid(), ProjectCode = "CDT-2026-001", ProjectName = "Mua thuốc chống dịch", ProcessBranchId = branchNhanh1.Id, CurrentStepOrder = 4, Status = "Completed", CreatedDate = DateTime.Now.AddDays(-4) };
            context.BiddingProjects.Add(p3);
            context.ProjectSteps.AddRange(
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p3.Id, ProcessStepId = n1_b1.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-4), CompletedDate = DateTime.Now.AddDays(-3) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p3.Id, ProcessStepId = n1_b2.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-3), CompletedDate = DateTime.Now.AddDays(-2) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p3.Id, ProcessStepId = n1_b3.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-2), CompletedDate = DateTime.Now.AddDays(-1), FormDataJson = "{\"ten_nha_thau\":\"Công ty Dược VN\",\"gia_trung_thau\":\"99000000\"}" },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p3.Id, ProcessStepId = n1_b4.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-1), CompletedDate = DateTime.Now }
            );

            // Case 4: Lỗi bị hủy tại Bước 2 do dự thảo hợp đồng sai
            var p4 = new BiddingProject { Id = Guid.NewGuid(), ProjectCode = "CDT-2026-002", ProjectName = "Mua văn phòng phẩm", ProcessBranchId = branchNhanh1.Id, CurrentStepOrder = 2, Status = "Cancelled", CreatedDate = DateTime.Now.AddDays(-2) };
            context.BiddingProjects.Add(p4);
            context.ProjectSteps.AddRange(
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p4.Id, ProcessStepId = n1_b1.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-2), CompletedDate = DateTime.Now.AddDays(-1) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p4.Id, ProcessStepId = n1_b2.Id, Status = "Processing", StartDate = DateTime.Now.AddDays(-1), Comments = "Dự thảo hợp đồng sai biểu mẫu của Bộ Y tế. Gói thầu bị hủy để làm lại." }, // Hủy tại đây
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p4.Id, ProcessStepId = n1_b3.Id, Status = "Pending" },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p4.Id, ProcessStepId = n1_b4.Id, Status = "Pending" }
            );


            // ================== NHÁNH 2: ĐẤU THẦU RỘNG RÃI ==================
            // Case 5: Mượt mà
            var p5 = new BiddingProject { Id = Guid.NewGuid(), ProjectCode = "DTRR-2026-001", ProjectName = "Xây dựng khu nhà C", ProcessBranchId = branchNhanh2.Id, CurrentStepOrder = 4, Status = "Completed", CreatedDate = DateTime.Now.AddDays(-6) };
            context.BiddingProjects.Add(p5);
            context.ProjectSteps.AddRange(
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p5.Id, ProcessStepId = n2_b1.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-6), CompletedDate = DateTime.Now.AddDays(-5) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p5.Id, ProcessStepId = n2_b2.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-5), CompletedDate = DateTime.Now.AddDays(-3) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p5.Id, ProcessStepId = n2_b3.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-3), CompletedDate = DateTime.Now.AddDays(-2) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p5.Id, ProcessStepId = n2_b4.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-2), CompletedDate = DateTime.Now.AddDays(-1) }
            );

            // Case 6: Kẹt tại bước Thẩm định E-HSMT
            var p6 = new BiddingProject { Id = Guid.NewGuid(), ProjectCode = "DTRR-2026-002", ProjectName = "Mua sắm trang thiết bị Y tế năm 2026", ProcessBranchId = branchNhanh2.Id, CurrentStepOrder = 2, Status = "InProgress", CreatedDate = DateTime.Now.AddDays(-3) };
            context.BiddingProjects.Add(p6);
            context.ProjectSteps.AddRange(
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p6.Id, ProcessStepId = n2_b1.Id, Status = "Completed", StartDate = DateTime.Now.AddDays(-3), CompletedDate = DateTime.Now.AddDays(-1) },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p6.Id, ProcessStepId = n2_b2.Id, Status = "Processing", StartDate = DateTime.Now.AddDays(-1), Comments = "Hồ sơ thiết kế chưa rõ ràng, yêu cầu bổ sung cấu hình kỹ thuật." },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p6.Id, ProcessStepId = n2_b3.Id, Status = "Pending" },
                new ProjectStep { Id = Guid.NewGuid(), BiddingProjectId = p6.Id, ProcessStepId = n2_b4.Id, Status = "Pending" }
            );

            await context.SaveChangesAsync();
            #endregion
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
