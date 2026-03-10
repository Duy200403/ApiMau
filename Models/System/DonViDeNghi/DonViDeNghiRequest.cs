using System.ComponentModel.DataAnnotations;

namespace ApiWebsite.Models.System.DonViDeNghi
{
    public class DonViDeNghiRequest
    {
        [Required(ErrorMessage = "Mã đơn vị không được để trống")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Tên đơn vị không được để trống")]
        public string Name { get; set; }

        // Thêm trường ParentId để phân cấp cha con
        public long? ParentId { get; set; }
    }
}
