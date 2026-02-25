using ApiWebsite.Common;
using ApiWebsite.Helper;
using System.Collections.Generic;

namespace ApiWebsite.Models
{
    public class AccountPagingFilter: PagingRequestBase
    {
         public string Keyword { get; set; }
         public Role? Role { get; set; }
        //  public List<string> LstAccountId { get; set; }
    }
}