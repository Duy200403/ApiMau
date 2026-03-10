using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiWebsite.Models.Bidding.ProjectStepAttachment
{
    public class ProjectStepAttachmentRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn Bước tiến độ")]
        public Guid ProjectStepId { get; set; }

        // LƯU Ý QUAN TRỌNG: Tên biến phải là 'Files' (có chữ s) và kiểu là List<IFormFile>
        [Required(ErrorMessage = "Vui lòng chọn ít nhất 1 file đính kèm")]
        public List<IFormFile> Files { get; set; }
    }
}