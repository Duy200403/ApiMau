using ApiWebsite.Common;
using System;

namespace ApiWebsite.Models.Bidding
{
    // Kế thừa PagingRequestBase đã có sẵn trong source của bạn
    public class BiddingProjectPagingFilter : PagingRequestBase
    {
        public string Keyword { get; set; }
        public string Status { get; set; }
        public Guid? ProcessBranchId { get; set; }
    }
}