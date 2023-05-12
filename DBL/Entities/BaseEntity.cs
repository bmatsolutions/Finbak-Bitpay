using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BITPay.DBL.Entities
{
    public class BaseEntity
    {
        [NotMapped]
        [JsonIgnore]
        public static string IDColumn { get { return "Id"; } }

        [NotMapped]
        [JsonProperty("stat")]
        public int RespStatus { get; set; }

        [NotMapped]
        [JsonProperty("msg")]
        public string RespMessage { get; set; }
    }
}
