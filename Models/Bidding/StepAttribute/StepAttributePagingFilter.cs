using ApiWebsite.Common;
using System;

namespace ApiWebsite.Models.Bidding.StepAttribute
{
    public class StepAttributePagingFilter : PagingRequestBase
    {
        // Rất quan trọng: Phải lọc theo ID của bước
        public Guid? ProcessStepId { get; set; }
        public string Keyword { get; set; }

    }
}