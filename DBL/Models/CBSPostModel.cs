using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class CBSPostModel
    {
        [JsonProperty("accountnocr")]
        public string CrAccount { get; set; }

        [JsonProperty("accountnodr")]
        public string DrAccount { get; set; }

        [JsonProperty("narration")]
        public string Narration { get; set; }

        [JsonProperty("currencycode")]
        public string CurrencyCode { get; set; }

        [JsonProperty("docreference")]
        public string RefNo { get; set; }

        [JsonProperty("txncode")]
        public string TxnCode { get; set; }

        [JsonProperty("Txntype")]
        public string TxnType { get; set; }

        [JsonProperty("officer")]
        public string Officer { get; set; }

        [JsonProperty("flag")]
        public string Flag { get; set; }

        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [JsonProperty("nameofdepositororwithdrawer")]
        public string TransactorName { get; set; }

        [JsonProperty("chqno")]
        public string ChequeNo { get; set; }


        [JsonProperty("Txndata")]
        public string Txndata { get; set; }

        [JsonProperty("DebitBr")]
        public string brachCode { get; set; }

        [JsonProperty("Appid")]
        public int Appid { get; set; }
    }

    public class CBSPostResposeModel
    {
        [JsonProperty("Successful")]
        public bool Successful { get; set; }

        [JsonProperty("Response")]
        public string Response { get; set; }

        [JsonProperty("Balance")]
        public string Balance { get; set; }

        [JsonProperty("Txnref")]
        public string Txnref { get; set; }

        [JsonProperty("BankRef")]
        public string BankRef { get; set; }
    }

    public class BalanceQueryResposeModel
    {
        [JsonProperty("Successful")]
        public bool Successful { get; set; }

        [JsonProperty("Response")]
        public string Response { get; set; }

        [JsonProperty("Balance")]
        public decimal Balance { get; set; }

        [JsonProperty("OdBalance")]
        public decimal Overdraft { get; set; }

        [JsonProperty("Accountname")]
        public string AccountName { get; set; }

        [JsonProperty("Branch")]
        public string Branch { get; set; }
    }

    public class ChequeQueryResposeModel
    {
        [JsonProperty("Valid")]
        public bool Valid { get; set; }

        [JsonProperty("Response")]
        public string Response { get; set; }

        [JsonProperty("CustomerName")]
        public string CustomerName { get; set; }
    }
}

