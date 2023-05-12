using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class TaxFileMiniModel
    {
        public int FileCode { get; set; }
        public DateTime CreateDate { get; set; }
        public int StatusCode { get; set; }
        public decimal TaxAmount { get; set; }
        public string OfficeName { get; set; }
        public string DclntName { get; set; }
        public string CompanyName { get; set; }
        public string AsmtNumber { get; set; }
        public string StatusName { get; set; }
    }
    public class DomesticTaxFileMiniModel
    {
        public int FileCode { get; set; }
        public DateTime CreateDate { get; set; }
        public int DeclNo { get; set; }
        public string Dr_Account { get; set; }
        public string Cr_Account { get; set; }
        public decimal Amount { get; set; }
        public decimal DomesticAmount { get; set; }
        public string CustomerName { get; set; }
        public string TaxPeriod { get; set; }
        public string ReceiptNo { get; set; }
        public string Mode { get; set; }
        public string TaxName { get; set; }
        public string TransactionType { get; set; }
        public string UserName { get; set; }
        public string StatusName { get; set; }
    }
}
