using System;

namespace ApiWebsite.Models.System.DonViDeNghi
{
    public class DonViDeNghiResponse
    {
        // Chú ý: Đổi thành kiểu long
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
