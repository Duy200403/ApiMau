using System;
using System.ComponentModel.DataAnnotations;

namespace ApiWebsite.Models.Bidding.ProcessStep
{
    public class ProcessStepRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn nhánh quy trình")]
        public Guid ProcessBranchId { get; set; }

        [Required(ErrorMessage = "Mã bước không được để trống")]
        public string StepCode { get; set; }

        [Required(ErrorMessage = "Tên bước không được để trống")]
        public string StepName { get; set; }

        [Required(ErrorMessage = "Thứ tự bước không được để trống")]
        public int Order { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Đơn vị thực hiện")]
        public long TargetDonViId { get; set; }

        public int SlaDays { get; set; }
        public bool RequiresAttachment { get; set; }
    }
}