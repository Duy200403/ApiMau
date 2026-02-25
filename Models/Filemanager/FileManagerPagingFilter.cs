using ApiWebsite.Common;

namespace ApiWebsite.Models
{
  public class FileManagerPagingFilter : PagingRequestBase
  {
    public string Keyword { get; set; }
  }
}