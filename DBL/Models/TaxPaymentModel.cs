using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class TaxPaymentModel
    {
        public string DclntName { get; set; }
        public string CompanyName { get; set; }
        public string AccountHolder { get; set; }
        public string CompanyNameMini { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public string CrAccountName { get; set; }
        public string CustAccountName { get; set; }
        public string PayModeName { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public int PaymentCode { get; set; }
        
        public string BrachCode { get; set; }
        public int Mode { get; set; } //used to identify multiple payments with one payment mode
        public int TaxType { get; set; }
        public string TaxRef { get; set; }
        public string TaxPeriod { get; set; }
        public string Details { get; set; }
        public string ReferenceNo { get; set; }
        public int ItemType { get; set; }
        public string DrAccount { get; set; }
        public string CrAccount { get; set; }
        public string TypeName { get; set; }

        [Required]
        public int FileCode { get; set; }

        [Required]
        [Display(Name = "Amount to Pay")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Mode of Payment")]
        public int PayMode { get; set; }
        public dynamic PaymentModes { get; set; }

        public string Remarks { get; set; }

        [Display(Name = "Print Receipt")]
        public bool PrintReceipt { get; set; }
       
        [Required]
        [Display(Name = "Account No")]
        public string AccountNo { get; set; }

        [Display(Name = "Waive Charge")]
        public bool NoCharge { get; set; }

        [Required]
        [Display(Name = "Received From")]
        public string ReceivedFrom { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public string SortCode { get; set; }
        public int BankCode { get; set; }
        [Required]
        [Display(Name = "Cheque Number")]
        public string ChequeNo { get; set; }



        public int PayType { get; set; }

        [Required]
        [Display(Name = "CBS Reference")]
        public string CBSRef { get; set; }

        public int PostToCBS { get; set; }
        public int RespStatus { get; set; }
        public string RespMessage { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionRef { get; set; }
        public string TaxPayerName { get; set; }
        public bool PayPartial { get; set; }
    }
}
