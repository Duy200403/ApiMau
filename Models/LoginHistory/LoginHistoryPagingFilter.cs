using ApiWebsite.Common;

namespace ApiWebsite.Models
{
    public class LoginHistoryPagingFilter : PagingRequestBase
    {
         public string Keyword { get; set; }
    }
}