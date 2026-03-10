using System;

namespace ApiWebsite.Models.Bidding.StepAttribute
{
    public class StepAttributeResponse
    {
        public Guid Id { get; set; }
        public Guid ProcessStepId { get; set; }
        public string ProcessStepName { get; set; } // Tên bước (Lấy qua liên kết mềm)

        public string AttributeCode { get; set; }
        public string AttributeName { get; set; }
        public int InputType { get; set; }
        public string SelectOptionsJson { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}