using System.ComponentModel.DataAnnotations;

namespace ApiWebsite.Models.Bidding.ProcessBranch
{
    public class ProcessBranchRequest
    {
        [Required(ErrorMessage = "Mã nhánh không được để trống")]
        public string BranchCode { get; set; }

        [Required(ErrorMessage = "Tên nhánh không được để trống")]
        public string BranchName { get; set; }

        public string Description { get; set; }
    }
}
