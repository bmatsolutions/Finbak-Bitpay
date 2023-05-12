using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
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

    public class FinBridgeApiRequest
    {
        [JsonProperty("tsp")]
        public string Timestamp { get; set; }

        [JsonProperty("ver")]
        public int version { get; set; }

        [JsonProperty("act")]
        public int action { get; set; }

        [JsonProperty("content")]
        public dynamic Data { get; set; }

        [JsonProperty("hdata")]
        public FinBridgeReqHeaderData Reference { get; set; }
    }

    public class FinBridgeReqHeaderData
    {
        [JsonProperty("ref_no")]
        public string Refno { get; set; }

        [JsonProperty("user_id")]
        public string UserID { get; set; }

        [JsonProperty("src_id")]
        public string SrcId { get; set; }
    }

    public class FinBridgeApiResponseModel
    {
        [JsonProperty("stat")]
        public int RespStatus { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public dynamic Data { get; set; }
        [JsonProperty("Success")]
        public dynamic Success { get; set; }
    }
}
