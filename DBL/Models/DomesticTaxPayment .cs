using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class DomesticTaxPayment
    {
        public string CustomerName { get; set; }
        public string tin { get; set; }
        public string Period { get; set; }
        public string DomesticTaxType { get; set; }
        public string TaxName { get; set; }
        public string DomesticTaxName { get; set; }
        public string TaxAdjustmentCat { get; set; }
        public string DeclNo { get; set; }
        public string commune { get; set; }

        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }

        public dynamic PaymentModes { get; set; }
        public decimal ChargeAmount { get; set; }
        public string CrAccountName { get; set; }
        public string CustAccountName { get; set; }
        public string PayModeName { get; set; }
        public int FileCode { get; set; }
        public string adjustment { get; set; }

        public string Delay { get; set; }

        [Display(Name = "Print Receipt")]
        public bool PrintReceipt { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Required]
        [Display(Name = "Mode of Payment")]
        public int PayMode { get; set; }

        [Required]
        [Display(Name = "Account No")]
        public string AccountNo { get; set; }

        [Required]
        [Display(Name = "Cheque Number")]
        public string ChequeNo { get; set; }

        [Display(Name = "Waive Charge")]
        public bool NoCharge { get; set; }

        [Required]
        [Display(Name = "Payer Name")]
        public string ReceivedFrom { get; set; }

        [Required]
        [Display(Name = "Bank")]
        public string SortCode { get; set; }

        public string CommuneName { get; set; }

        public string ReceiptNo { get; set; }
        public string UserName { get; set; }
        public string Mode { get; set; }
        [Required]
        [Display(Name = "Cr_Account")]
        public string Cr_Account { get; set; }

        [Required]
        [Display(Name = "Dr_Account")]
        public string Dr_Account { get; set; }
        [Required]
        [Display(Name = "CBS Reference")]
        public string CBSref { get; set; }
        public int PostToCBS { get; set; }
        public int RespStat { get; set; }
        public string RespMessage { get; set; }
        public string Chasis { get; set; }
        public string Imma { get; set; }
        public string CarOnwer { get; set; }
        public string Contracavation { get; set; }
        public string Document { get; set; }
        public string DriverName { get; set; }
        public string Education { get; set; }
        public string Infraction { get; set; }
        public string LicenceType { get; set; }
        public string Service { get; set; }
        public string Copies { get; set; }
        public string Vehicle { get; set; }
        public string Word { get; set; }
        public string Product { get; set; }
        public string BankCode { get; set; }
        public DateTime CreateDate { get; set; }
        public string CNI { get; set; }
        public string CNIIssue { get; set; }
        public string ManufactureYear { get; set; }
        public string BrandType { get; set; }
    }
}
