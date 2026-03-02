using System;

namespace ApiWebsite.Models.Bidding.ProcessBranch
{
    public class ProcessBranchResponse
    {
        public Guid Id { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Description { get; set; }
    }
}
