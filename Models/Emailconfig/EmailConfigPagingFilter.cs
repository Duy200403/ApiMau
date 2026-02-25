using ApiWebsite.Common;

namespace ApiWebsite.Models
{
    public class EmailConfigPagingFilter : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}