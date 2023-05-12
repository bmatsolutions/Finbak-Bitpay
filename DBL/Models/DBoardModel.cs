using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class DBoardModel
    {
        [JsonProperty("allFiles")]
        public int AllFiles { get; set; }

        [JsonProperty("pending")]
        public int FilesPending { get; set; }

        [JsonProperty("completed")]
        public int FilesCompleted { get; set; }

        [JsonProperty("failed")]
        public int FailedFiles { get; set; }

        [JsonProperty("approval")]
        public int ApprovalCount { get; set; }

        [JsonProperty("topupapproval")]
        public int Topupapproval { get; set; }

        [JsonProperty("tokenapproval")]
        public int Tokenapproval { get; set; }

        [JsonProperty("alltopups")]
        public int Alltopups { get; set; }

        [JsonProperty("alltokenpurchase")]
        public int alltokenpurchase { get; set; }

        [JsonProperty("domesticapproval")]
        public int Domesticapproval { get; set; }

        [JsonProperty("mairieapproval")]
        public int Mairieapproval { get; set; }
    }
}
