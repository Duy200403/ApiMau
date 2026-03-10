using ApiWebsite.Common;
using System;

namespace ApiWebsite.Models.Bidding.ProcessStep
{
    public class ProcessStepPagingFilter : PagingRequestBase
    {
        public Guid? ProcessBranchId { get; set; }
        public string Keyword { get; set; }

    }
}
