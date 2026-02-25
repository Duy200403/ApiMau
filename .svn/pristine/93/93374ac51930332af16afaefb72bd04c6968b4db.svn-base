using System.Collections.Generic;

namespace ApiWebsite.Models.Response
{
    public class ErrorResponseModel
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public System.Net.HttpStatusCode Status { get; set; } = System.Net.HttpStatusCode.BadRequest;
        public string TraceId { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}