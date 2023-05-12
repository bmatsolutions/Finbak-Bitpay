using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBRClient.Models
{
    public class LookUpRequestData
    {
        [JsonProperty("tcode")]
        public string TransactionCode { get; set; }
    }

    public class ValidateTinRequestData
    {
        [JsonProperty("tin")]
        public string tin { get; set; }
    }
    public class ValidateNIFRequestData
    {
        [JsonProperty("nif")]
        public string nif { get; set; }
    }
    public class ValidateDeclarantRequestData
    {
        [JsonProperty("declarant")]
        public string declarant { get; set; }
    }
}