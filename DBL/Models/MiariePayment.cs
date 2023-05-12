using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class MiariePaymentModel
    {
        public int TaxNoteType { get; set; }
        public string TaxNoteNo { get; set; }
        public string RefNo { get; set; }
        public string TaxPayerName { get; set; }
        public string Descr { get; set; }
        public string Period { get; set; }
        public string CrAccountName { get; set; }
        public string CustAccountName { get; set; }
        public string PayModeName { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PayDate { get; set; }
        public int PaymentCode { get; set; }
        public int BankCode { get; set; }
        public string BrachCode { get; set; }
        public int FileCode { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public int PayMode { get; set; }
        public string Remarks { get; set; }
        public string ChequeNo { get; set; }
        public string ReceivedFrom { get; set; }
        public string SortCode { get; set; }
        public string TaxName { get; set; }
        public string Cr_Account { get; set; }
        public string Dr_Account { get; set; }
        public string TypeName { get; set; }

        public int RespStat { get; set; }
        public string RespMessage { get; set; }
    }
}
