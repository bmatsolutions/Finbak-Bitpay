using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class FailedTransactions
    {
        public string Year { get; set; }
        public string RegNo { get; set; }
        public string RegSerial { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string StatMsg { get; set; }
        public string OfficeCode { get; set; }
        public string OfficeName { get; set; }
        public string DeclCode { get; set; }
        public string DeclName { get; set; }
        public string Nif { get; set; }
        public string CompName { get; set; }
        public DateTime CreateDate { get; set; }
        public string CrAccount { get; set; }
        public string DrAccount { get; set; }
        public string Remarks { get; set; }
        public string CBSRef { get; set; }
        public string FileCode { get; set; }
    }
}
