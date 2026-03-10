using System;

namespace ApiWebsite.Models.Bidding.ProjectStep
{
    public class ProjectStepResponse
    {
        public Guid Id { get; set; }
        public Guid BiddingProjectId { get; set; }
        public Guid ProcessStepId { get; set; }

        public string ProcessStepName { get; set; } // Lấy qua liên kết mềm
        public int StepOrder { get; set; }          // Thứ tự bước

        public string Status { get; set; } // Pending, Processing, Completed
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletedByUserId { get; set; }

        public string FormDataJson { get; set; } // Chứa dữ liệu người dùng nhập
        public string Comments { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}