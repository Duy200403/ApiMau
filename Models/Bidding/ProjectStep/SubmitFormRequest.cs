using System;
using System.ComponentModel.DataAnnotations;

namespace ApiWebsite.Models.Bidding.ProjectStep
{
    public class SubmitFormRequest
    {
        [Required]
        public bool IsCompleted { get; set; } // true = Hoàn thành & Chuyển bước, false = Chỉ lưu nháp

        public string Comments { get; set; }  // Ghi chú thêm nếu có

        public object FormData { get; set; }  // Hứng cục JSON dữ liệu form động
    }
    public class SwitchBranchRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn nhánh quy trình mới")]
        public Guid NewBranchId { get; set; }
    }
}