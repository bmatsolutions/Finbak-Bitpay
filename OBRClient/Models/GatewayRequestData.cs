using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OBRClient.Models
{
    public class GatewayRequestData
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("uname")]
        public string UserName { get; set; }

        [JsonProperty("pwd")]
        public string Password { get; set; }

        [JsonProperty("fctn")]
        public int Function { get; set; }

        [JsonProperty("data")]
        public dynamic Data { get; set; }
    }
}