using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BITPayService.Models
{
    public class FinBridgeAuthResult
    {
        [JsonProperty("auth_token")]
        public string Token { get; set; }

        [JsonProperty("expiry")]
        public string Expiry { get; set; }

        [JsonProperty("err_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("err_msg")]
        public string ErrorMsg { get; set; }
    }

    public class ApiResponseModel
    {
        [JsonProperty("stat")]
        public int RespStatus { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public dynamic Data { get; set; }
    }

    public class ApiResultBaseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }
}
