using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class RegidesoModel
    {
        [Display(Name = "Status")]
        [JsonProperty("stat")]
        public int stat { get; set; }
        
        [Display(Name = "Message")]
        [JsonProperty("msg")]
        public string Msg { get; set; }
        public string RespMessage { get; set; }
        public bool Success { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        [JsonProperty("accnt_no")]
        public string Accnt_no { get; set; }
        [JsonProperty("records")]
        public int Records { get; set; }
        [JsonProperty("morerecords")]
        public bool Morerecords { get; set; }
        [JsonProperty("bills")]
        public Bills[] bills { get; set; }
        [JsonProperty("data")]
        public dynamic Data { get; set; }
        
    }
    public class Bills
    {
        [JsonProperty("error_code")]
         public string error_code { get; set; }
         [Display(Name = "Status")]
        public int stat { get; set; }
        [JsonProperty("clientname")]
        public string ClientName { get; set; }

        [Display(Name = "Message")]
        [JsonProperty("msg")]
        public string Msg { get; set; }
        [JsonProperty("data")]
        public dynamic Data { get; set; }
        [JsonProperty("paid_amount")]
        public decimal Amnt_paid { get; set; }
        [JsonProperty("amount")]
         public decimal Amount { get; set; }
        [JsonProperty("invoice_amnt")]
        public decimal invoice_amnt { get; set; } 
        [JsonProperty("deduction_amnt")]
        public decimal deduction_amnt { get; set; }
        [JsonProperty("installation_code")]
        public string installation_code { get; set; }
        [JsonProperty("amnt")]
        public decimal Amnt { get; set; }
        [JsonProperty("received_code")]
        public int received_code { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        [JsonProperty("accnt_no")]
        public string Accnt_no { get; set; }
        [JsonProperty("cust_no")]
        public string Cust_no { get; set; }
        [JsonProperty("year")]
        public int Year { get; set; }
        [Display(Name = "Account Number")]
        public string DrAccount { get; set; }
        [Display(Name = "Invoice Number")]
        [JsonProperty("month")]
        public int Month { get; set; }
        [JsonProperty("invoice_no")]
        public string Invoice_no { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        [JsonProperty("cust_name")]
        public string Cust_name { get; set; }
        [JsonProperty("activity")]
        public string Activity { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNo { get; set; }
        [Required]
        [Display(Name = "Mode of Payment")]
        public int PayMode { get; set; }
        public int BillCode { get; set; }
        public dynamic PaymentModes { get; set; }
        public string PayModeName { get; set; }
        [Display(Name = "Bank")]
        public string SortCode { get; set; }
        public int BankCode { get; set; }
        [Display(Name = "Cheque Number")]
        public string ChequeNo { get; set; }
        public string txnId { get; set; }
        public bool Success { get; set; }
        public int Maker { get; set; }
        public string ReceivedFrom { get; set; }
        public bool NeedApproval { get; set; }
        public string CBSRef { get; set; }
        public string UserName { get; set; }
        public string Remarks { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }


    }
    public class PrePaidModel
    {
        [Required]
        [Display(Name = "Meter Number")]
        public string Meter_No { get; set; }

        [Display(Name = "Amount")]
        public decimal Amnt { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Account Number")]
        public string Accnt_no { get; set; }
        [JsonProperty("cust_code")]
        public string Cust_no { get; set; }
        public int Year { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNo { get; set; }
        public int Month { get; set; }
        public string Invoice_no { get; set; }
        public string CBSRef { get; set; }
        public string Cust_name { get; set; }
        [Display(Name = "Account Number")]
        public string DrAccount { get; set; }
        public string Activity { get; set; }
        public int Stat { get; set; }
        public int Maker { get; set; }
        public int BillCode { get; set; }
        public int units { get; set; }
        public string Msg { get; set; }
        public string Data { get; set; }
        public string ReceivedFrom { get; set; }
        public string Remarks { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("token1")]
        public string Token1 { get; set; }
        [JsonProperty("token2")]
        public string Token2 { get; set; }
        [JsonProperty("token3")]
        public string Token3 { get; set; }
        [JsonProperty("receivedCode")]
        public int receivedCode { get; set; }
        [JsonProperty("checkoutsite")]
        public string CheckoutSite { get; set; }
        [JsonProperty("ErrorCode")]
        public string ErrorCode { get; set; }
        [JsonProperty("payamount")]
        public decimal PayAmount { get; set; }
        [JsonProperty("consumption")]
        public decimal Consumption { get; set; }
        [JsonProperty("advancecredit")]
        public decimal AdvanceCredit { get; set; }
        [JsonProperty("royalty")]
        public decimal Royalty { get; set; }
        [JsonProperty("primefixed")]
        public decimal PrimeFixed { get; set; }
        [JsonProperty("amountfine")]
        public decimal AmountFined { get; set; }
        [JsonProperty("surcharge")]
        public decimal SurCharge { get; set; }
        [JsonProperty("tax")]
        public decimal Tax { get; set; }
        [JsonProperty("vat")]
        public decimal vat { get; set; }
        public bool NeedApproval { get; set; }
        public decimal AmountToken { get; set; }
        public string Token { get; set; }

       [JsonProperty("lastreadingdate")]
        public string Lastreadingdate { get; set; }
        [JsonProperty("newreadingdate")]
        public string Newreadingdate { get; set; }

        
        public int Cust_Code { get; set; }
        [JsonProperty("cust_name")]
        public string Cust_Name { get; set; }
        [Required]
        [Display(Name = "Mode of Payment")]
        public int PayMode { get; set; }
        [Display(Name = "Mode of Payment")]
        public string PayModeName { get; set; }
        public dynamic PaymentModes { get; set; }
        [Display(Name = "Bank")]
        public string SortCode { get; set; }
        public int BankCode { get; set; }
        [Display(Name = "Cheque Number")]
        public string ChequeNo { get; set; }
        public string UserName { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }

    }

    public class QueryDetails
    {
    [Display(Name = "Meter Number")]
        public string Meter_No { get; set; }
        [JsonProperty("amnt")]
        public decimal Amount { get; set; }
        [Display(Name = "units")]
        public string units { get; set; }
        [JsonProperty("cust_name")]
        public string CustName { get; set; }
        public string ClientName { get; set; }
        [JsonProperty("cust_code")]
        public string CustCode { get; set; }
        [JsonProperty("message")]
        public string Msg { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }


    }
}
