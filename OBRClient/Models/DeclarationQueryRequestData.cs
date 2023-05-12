using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace OBRClient.Models
{
    public class DeclarationQueryRequestData
    {
        [JsonProperty("assNo")]
        public string AssessmentNumber { get; set; }

        [JsonProperty("assSerial")]
        public string AssessmentSerial { get; set; }

        [JsonProperty("offCode")]
        public string OfficeCode { get; set; }

        [JsonProperty("regNo")]
        public string RegistrationNumber { get; set; }

        [JsonProperty("regSerial")]
        public string RegistrationSerial { get; set; }

        [JsonProperty("regYear")]
        public string RegistrationYear { get; set; }
    }

    public class CreditQueryRequestData
    {
        [JsonProperty("accRef")]
        public string AccountReference { get; set; }

        [JsonProperty("offCode")]
        public string OfficeCode { get; set; }

        [JsonProperty("refYear")]
        public string ReferenceYear { get; set; }

        [JsonProperty("refNo")]
        public string ReferenceNumber { get; set; }
    }

    public class OtherQueryRequestData
    {
        [JsonProperty("tranCode")]
        public string TransactionCode { get; set; }
        [JsonProperty("amt")]
        public decimal Amount { get; set; }
        [JsonProperty("currCode")]
        public string Currency { get; set; }
    }
}
