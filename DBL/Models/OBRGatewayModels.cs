using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
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

    public class AddTranRequestData
    {
        [JsonProperty("tcode")]
        public string TransactionCode { get; set; }

        [JsonProperty("ttype")]
        public string TransactionType { get; set; }

        [JsonProperty("date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("cusname")]
        public string CustomerName { get; set; }

        [JsonProperty("payername")]
        public string PayerName { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("other")]
        public string OtherFields { get; set; }

        [JsonProperty("tin")]
        public string Tin { get; set; }
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
    public class ValidateDecRequestData
    {
        [JsonProperty("declarant")]
        public string declarant { get; set; }
    }
    public class QueryDomesticPayment
    {
        [JsonProperty("tcode")]
        public string TransactionCode { get; set; }
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
