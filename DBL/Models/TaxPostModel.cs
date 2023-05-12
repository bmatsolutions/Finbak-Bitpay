using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class TaxPostModel
    {
        public int PaymentMode { get; set; }
        public bool Charge { get; set; }
        public string CurrencyCode { get; set; }
        public string Officer { get; set; }
        public string TransactorName { get; set; }
        public PostTxnData MainTransaction { get; set; }
        public PostTxnData ChargeTransaction { get; set; }
    }

    public class PostTxnData
    {
        public string CrAccount { get; set; }
        public string DrAccount { get; set; }
        public string Narration { get; set; }
        public string RefNo { get; set; }
        public string TxnCode { get; set; }
        public string TxnType { get; set; }
        public string Officer { get; set; }
        public string Flag { get; set; }
        public decimal Amount { get; set; }
        public string Url { get; set; }
    }

    
}
