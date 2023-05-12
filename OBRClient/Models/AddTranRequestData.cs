using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBRClient.Models
{
    public class AddTranRequestData
    {
        [JsonProperty("tcode")]
        public string TransactionCode { get; set; }

        [JsonProperty("ttype")]
        public string TransactionType { get; set; }

        [JsonProperty("date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("payername")]
        public string PayerName { get; set; }

        [JsonProperty("cusname")]
        public string CustomerName { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("other")]
        public string OtherFields { get; set; }

        [JsonProperty("tin")]
        public string Tin { get; set; }
    }
}
