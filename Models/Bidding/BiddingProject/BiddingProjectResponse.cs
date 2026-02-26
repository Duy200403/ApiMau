using System;

namespace ApiWebsite.Models.Bidding
{
    public class BiddingProjectResponse
    {
        public Guid Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public int CurrentStepOrder { get; set; }
        public string BranchName { get; set; } // Lấy từ bảng ProcessBranch qua
    }
}