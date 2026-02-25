using System;

namespace ApiWebsite.Models
{
    public class LoginHistoryRespose
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime LoginDate { get; set; }
        public Guid AccountId { get; set; }
    }
}