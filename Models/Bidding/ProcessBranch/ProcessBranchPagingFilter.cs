using ApiWebsite.Common;

namespace ApiWebsite.Models.Bidding.ProcessBranch
{
    public class ProcessBranchPagingFilter : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
