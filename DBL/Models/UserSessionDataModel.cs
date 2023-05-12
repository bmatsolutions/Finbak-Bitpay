using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class UserSessionDataModel
    {
        [JsonProperty("ocnt")]
        public int OrdersCount { get; set; }
    }
}
