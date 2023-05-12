using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class ApprovalItemModel
    {
        public int ItemCode { get; set; }
        public DateTime CreateDate { get; set; }
        public string Details { get; set; }
        public int TypeCode { get; set; }
        public string ActionTypeName { get; set; }
        public string Maker { get; set; }
        public int LogId { get; set; }
    }

    public class ApprovalParamsModel
    {
        public int idxno { get; set; }
        public DateTime Date { get; set; }
        public string ItemName { get; set; }
        public string Descr { get; set; }
        public int OldItemValue { get; set; }
        public int NewItemValue { get; set; }
        public string Maker { get; set; }
        public int LogId { get; set; }
    }

    public class ApprovedPayments
    {
        public int Code { get; set; }
        public string Receipt_No { get; set; }
        public string DeclarantName { get; set; }
        public string DeclarantNo { get; set; }
        public string ReceiptNo { get; set; }
        public string PaymentMode { get; set; }
        public string DepositorName { get; set; }
        public string BranchCode { get; set; }
        public string Cr_Account { get; set; }
        public string Dr_Account { get; set; } 
        public string Amount { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string PaymentType { get; set; }
        public DateTime PaymentDate { get; set; }
    }


}
