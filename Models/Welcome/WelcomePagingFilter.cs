using System.Collections.Generic;
using ApiWebsite.Common;
using ApiWebsite.Helper;

namespace ApiWebsite.Models
{
    public class WelcomePagingFilter : PagingRequestBase
    {
        public string Keyword { get; set; }
        // public List<string> searchSTT { get; set; }

        public bool IsSortedNameAlphabet { get; set; } = false;
    }
}