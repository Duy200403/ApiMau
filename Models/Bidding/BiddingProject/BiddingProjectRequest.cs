using System;
using System.ComponentModel.DataAnnotations;

namespace ApiWebsite.Models.Bidding
{
    public class BiddingProjectRequest
    {
        [Required(ErrorMessage = "Mã không được để trống")]
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public Guid ProcessBranchId { get; set; }
    }
}