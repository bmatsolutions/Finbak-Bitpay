using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OBRClient.Models
{
    public class RequestResponseModel
    {
        //0-Success, 1-Failed, 2-Exception, 3-Webexception
        public int Status { get; set; }
        public string Message { get; set; }
        public dynamic Content { get; set; }
    }

    public class CreditStateData
    {
        public string OfficeName { get; set; }
        public string AssYear { get; set; }
        public string AssSerial { get; set; }
        public string AssNo { get; set; }
        public Decimal Amount { get; set; }
    }

    public class Tin
    {
        [JsonProperty("pName")]
        public string CustomerName { get; set; }
    }
    public class Nif
    {
        [JsonProperty("comp_name")]
        public string CompanyName { get; set; }
    }
    public class Declarant
    {
        [JsonProperty("DeclarantName")]
        public string declarantName { get; set; }
    }
    public class Payment
    {
        [JsonProperty("resId")]
        public string ResponseCode { get; set; }
    }

    public class TranLookUp
    {
        [JsonProperty("code")]
        public string TransactionCode { get; set; }
        [JsonProperty("cusName")]
        public string CustomerName { get; set; }
        [JsonProperty("date")]
        public string TransactionDate { get; set; }
        [JsonProperty("tranStat")]
        public string TransactionStatus { get; set; }
        
    }

    public class CreditQueryResponse
    {
        [JsonProperty("accHolder")]
        public string AccountHolder { get; set; }
        [JsonProperty("cmpyName")]
        public string CompanyName { get; set; }
        [JsonProperty("offCode")]
        public string OfficeCode { get; set; }
        [JsonProperty("offName")]
        public string OfficeName { get; set; }
        [JsonProperty("ttAmt")]
        public string TotalAmount { get; set; }
        [JsonProperty("recNo")]
        public string ReceiptNo { get; set; }
        [JsonProperty("recSerial")]
        public string ReceiptSerial { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
    }

    

    public class FetchTaxList
    {
        [JsonProperty("taxList")]
        public List<TaxList> TaxLists { get;set; }
    }

    public class TaxList
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
