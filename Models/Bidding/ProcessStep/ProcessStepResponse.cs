using System;

namespace ApiWebsite.Models.Bidding.ProcessStep
{
    public class ProcessStepResponse
    {
        public Guid Id { get; set; }
        public Guid ProcessBranchId { get; set; }
        public string BranchName { get; set; } // Lấy thêm tên nhánh cho FE đỡ phải query

        public string StepCode { get; set; }
        public string StepName { get; set; }
        public int Order { get; set; }

        public long TargetDonViId { get; set; }
        public string TargetDonViName { get; set; } // Lấy thêm tên phòng ban

        public int SlaDays { get; set; }
        public bool RequiresAttachment { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}