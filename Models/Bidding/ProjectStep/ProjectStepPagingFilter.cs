using ApiWebsite.Common;
using System;

namespace ApiWebsite.Models.Bidding.ProjectStep
{
    public class ProjectStepPagingFilter : PagingRequestBase
    {
        // Lọc để lấy danh sách các Bước thuộc 1 Gói thầu cụ thể
        public Guid? BiddingProjectId { get; set; }
        public string Keyword { get; set; }
        public string Status { get; set; }
    }
}
