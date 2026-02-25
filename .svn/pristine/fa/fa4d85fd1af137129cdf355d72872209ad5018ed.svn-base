using ApiWebsite.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ApiWebsite.Models
{

    [Table("Account")]
    // [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public class Account : AuditEntity<Guid>
    {
        public Account()
        {
            AccessFailedCount = 0;
            IsLock = false;
            IsActive = false;
        }
        //Username Tài khoản đăng nhập (Bắt buộc)
        [Required(ErrorMessage = "Tài khoản không được trống")]
        [Column(TypeName = "NVARCHAR(250)")]
        public string Username { get; set; }
        [Column(TypeName = "NVARCHAR(500)")]

        //PasswordHash Mật khẩu đã mã hóa
        public string PasswordHash { get; set; }
        [Column(TypeName = "NVARCHAR(1000)")]

        //PhoneNumber số điện thoại người dùng (không bắt buộc)
        public string PhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(1000)")]

        //Pseudonym bút danh người dùng
        public string Pseudonym { get; set; }

        //FullName tên người dùng

        [Column(TypeName = "NVARCHAR(250)")]
        public string FullName { get; set; }

        // [Column(TypeName = "NVARCHAR(250)")]
        public long DonViDeNghiId { get; set; }

        //Roles danh sách quyền của người dùng
        [Column(TypeName = "NVARCHAR(1000)")]
        public string Roles { get; set; }

   
        //Email thông tin email người dùng
        // [EmailAddress(ErrorMessage = "Không đúng định dạng Email")]
        [Column(TypeName = "NVARCHAR(100)")]
        public string Email { get; set; }


        //Salt Trường dữ liệu chưa biết làm gì
        [Column(TypeName = "NVARCHAR(250)")]
        public string Salt { get; set; }

        //IsActive Đánh dấu tài khoản đã được kích hoạt hay chưa
        public bool IsActive { get; set; }
        [Column(TypeName = "NVARCHAR(100)")]

        //TimeLock Thời gian tài khoản tạm bị khóa
        public DateTime? TimeLock { get; set; }
        
        // AccessFailedCount đánh dấu số lần đăng nhập. nếu quá 5 lần thì tài khoản bị khóa
        public int AccessFailedCount { get; set; } = 0;

        //IsLock đánh dấu tài khoản tạm thời bị khóa (5phut)
        public bool IsLock { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]

        // VerifacationCode gửi mã code để check lấy lại mật khẩu
        public string VerifacationCode { get; set; }

        // LoginHistory bảng quan hệ ghi lại lịch sử đăng nhập của tài khoản
        public virtual ICollection<LoginHistory> LoginHistory { get; set; }
        // [JsonIgnore]
        // public ICollection<BookDevice> BookDevices { get; set; }
    }
}