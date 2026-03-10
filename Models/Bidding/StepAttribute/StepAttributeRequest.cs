using System;
using System.ComponentModel.DataAnnotations;

namespace ApiWebsite.Models.Bidding.StepAttribute
{
    public class StepAttributeRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn Bước quy trình")]
        public Guid ProcessStepId { get; set; }

        [Required(ErrorMessage = "Mã thuộc tính không được để trống")]
        public string AttributeCode { get; set; }

        [Required(ErrorMessage = "Tên thuộc tính không được để trống")]
        public string AttributeName { get; set; }

        // 1: Text, 2: Number, 3: Date, 4: Dropdown...
        [Required(ErrorMessage = "Loại nhập liệu không được để trống")]
        public int InputType { get; set; }

        public string SelectOptionsJson { get; set; } // Dùng chứa mảng option nếu InputType là Dropdown
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
    }
}