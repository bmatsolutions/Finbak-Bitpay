using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OBRClient.Models
{
    public class BulkData
    {
        [JsonProperty("assNo")]
        public string AssessmentNumber { get; set; }

        [JsonProperty("assSerial")]
        public string AssessmentSerial { get; set; }

        [JsonProperty("offCode")]
        public string OfficeCode { get; set; }

        [JsonProperty("regNo")]
        public string RegistrationNumber { get; set; }

        [JsonProperty("regSerial")]
        public string RegistrationSerial { get; set; }

        [JsonProperty("regYear")]
        public string RegistrationYear { get; set; }
        [JsonProperty("amntToPay")]
        public string AmountToBePaid { get; set; }
        [JsonProperty("curr")]
        public string Currency { get; set; }
        [JsonProperty("tranCode")]
        public string TransactionCode { get; set; }
        [JsonProperty("tranRef")]
        public string TransactionReference { get; set; }
    }

    public class MakePaymentRequestModel : DeclarationQueryRequestData 
    {
        [JsonIgnore]
        public int FileCode { get; set; }

        [JsonProperty("tranId")]
        public string TranID { get; set; }

        [JsonProperty("declList")]
        public List<BulkData> DeclarationList { get; set; }

        [JsonProperty("dltCode")]
        public string DeclarantCode { get; set; }

        [JsonProperty("cmpyCode")]
        public string CompanyCode { get; set; }

        [JsonProperty("amntToPay")]
        public string AmountToBePaid { get; set; }

        [JsonProperty("payMode")]
        public string MeanOfPayment { get; set; }

        [JsonProperty("bCode")]
        public string BankCode { get; set; }

        [JsonProperty("checkRef")]
        public string CheckReference { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("payDate")]
        public string PaymentDate { get; set; }

        [JsonProperty("refund")]
        public string Refund { get; set; }

        [JsonProperty("payType")]
        public string Pay { get; set; }

        [JsonProperty("accHolder")]
        public string AccountHolder { get; set; }

        [JsonProperty("accRef")]
        public string AccountReference { get; set; }

        [JsonProperty("refYear")]
        public string ReferenceYear { get; set; }

        [JsonProperty("refNo")]
        public string ReferenceNumber { get; set; }
        public string TaxPayerName { get;  set; }
        public string TransactionCode { get;  set; }
        public string ReferenceOffice { get;  set; }
        public string TransactionReference { get;  set; }
        public string Currency { get;  set; }
    }

    public class DomesticTaxPayment
    {
        public string CustomerName { get; set; }
        public string tin { get; set; }
        public string Period { get; set; }
        public string DomesticTaxType { get; set; }
        public string TaxName { get; set; }
        public string DomesticTaxName { get; set; }
        public string TransactionType { get; set; }
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
        public int PaymentCode { get; set; }
        public bool PrintReceipt { get; set; }

        public string Remarks { get; set; }
        public int PayMode { get; set; }

        public string AccountNo { get; set; }

        public string ChequeNo { get; set; }

        public string ReceivedFrom { get; set; }

        public string SortCode { get; set; }

        public string CommuneName { get; set; }

        public string ReceiptNo { get; set; }
        public string UserName { get; set; }
        public string Mode { get; set; }

        public string Cr_Account { get; set; }

        public string Dr_Account { get; set; }

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
    public class DomesticPaymentResponse
    {
        public int id { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public PaymentResponseDetails content { get; set; }
    }
    public enum OBRFunctionType
    {
        QueryTax = 0,
        TaxPayment = 1,
        LookUp = 2,
        AddTran = 3,
        ValidateTin = 4,
        QueryCredit = 5,
        QueryTranRef = 6,
    }
    public enum SettingType
    {
        QueryTax = 0,
        TaxPayment = 1,
        DomesticPayment = 4,
        DomesticLookup = 5,
        Miarie = 6,
        QueryCredit = 7,
        QueryCBS = 8,
    }
    public class PaymentResponseDetails
    {
        public int id { get; set; }
        public string resId { get; set; }
    }
}
