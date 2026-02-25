using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiWebsite.Models.Base;
using System.Collections.Generic;
using ApiWebsite.Helper;
using Newtonsoft.Json;

namespace ApiWebsite.Models
{
    public class OriginalAccount
    {
        public OriginalAccount()
        {

        }
        [JsonProperty("NhanVienID")]
        public int NhanVienID { get; set; }
        [JsonProperty("UserName")]
        public string UserName { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("CreatedDate")]
        public string CreatedDate { get; set; }
        [JsonProperty("CreatedBy")]
        public string CreatedBy { get; set; }
        [JsonProperty("UpdatedDate")]
        public string UpdatedDate { get; set; }
        [JsonProperty("UpdatedBy")]
        public string UpdatedBy { get; set; }
        [JsonProperty("IsLocked")]
        public bool IsLocked { get; set; }
        [JsonProperty("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
}