using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace BITPay.DBL.Models
{
    public class ReportModel
    {
        public int ReportCode { get; set; }

        [Display(Name = "Report Name")]
        public string ReportTitle { get; set; }
    }

    public class BaseReportModels
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public string CustomMessage { get; set; }
        public string DateRange { get; set; }
        public string ReportName { get; set; }
    }

    public class MiarieReportModels : BaseReportModels
    {
        public string ReceiptNo { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string TaxName { get; set; }
        public string NoteNo{ get; set; }
        public string PayerName { get; set; }
        public Decimal SrcAmount { get; set; }
        public Decimal ReceivedAmount { get; set; }
        public string Remarks { get; set; }
        public string AmountWords { get; set; }
        public int PaymentCode { get; set; }
        public string Period { get; set; }
        public string PaymentMode { get; set; }
        public string BranchName { get; set; }
        public string Descr { get; set; }
        public string ReferenceNo { get; set; }
        //public string DeclarantDetails { get; set; }
        //public string TaxDetails { get; set; }
        public string ReceivedFrom { get; set; }
        //public string DeclarationNo { get; set; }
        public int StatusCode { get; set; }
        //public string OBRRefNo { get; set; }
        //public string Nif { get; set; }
        //public string DeclarantNo { get; set; }
    }


    public class BuyTokenReportModels : BaseReportModels
    {
        public string BillCode { get; set; }
        public string MeterNo { get; set; }
        public string Meter_No { get; set; }
        public string UserName { get; set; }
        public string AmountWords { get; set; }
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public decimal PayAmount { get; set; }
        public decimal Consumption { get; set; }
        public decimal AdvanceCredit { get; set; }
        public decimal Royalty { get; set; }
        public decimal PrimeFixed { get; set; }
        public decimal AmountFine { get; set; }
        public decimal SurCharge { get; set; }
        public decimal Tax { get; set; }
        public decimal Vat { get; set; }
        public string LastReadingDate { get; set; }
        public string NewReadingDate { get; set; }
        public string CustName { get; set; }
        public int CustCode { get; set; }
        public string Token1 { get; set; }
        public string Token2 { get; set; }
        public string Token3 { get; set; }
        public int ReceivedCode { get; set; }
        public string Maker { get; set; }
        public string checkoutsite { get; set; }
        public string ErrorCode { get; set; }
        public string PayModeName { get; set; }
        public string ReceivedFrom { get; set; }
        public int PayMode { get; set; }
        public string BranchName { get; set; }
        public string CBSRef { get; set; }
    }
    public class PostPayReportModels : BaseReportModels
    {
        public string InvoiceNo { get; set; }
        public string PhoneNo { get; set; }
        public string AmountWords { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal DeductionAmount { get; set; }
        public int Stat { get; set; }
        public int MyAction { get; set; }
        public int BillCode { get; set; }
        public string Msg { get; set; }
        public string Reason { get; set; }
        public decimal PaidAmount { get; set; }
        public int ReceivedCode { get; set; }
        public string ClientName { get; set; }
        public string InstallationCode { get; set; }
        public string UserName { get; set; }
        public string PayMode { get; set; }
        public string DRAccount { get; set; }
        public string PayModeName { get; set; }
        public string Remarks { get; set; }
        public string ReceivedFrom { get; set; }
        public string SortCode { get; set; }
        public int BankCode { get; set; }
        public string ChequeNo { get; set; }
        public string txnId { get; set; }
        public bool Success { get; set; }
        public int Maker { get; set; }
        public int RespStat { get; set; }
        public string RespMessage { get; set; }
        public bool NeedApproval { get; set; }
        public string CBSRef { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string BranchName { get; set; }

    }

    public class DomesticReportModels : BaseReportModels
    {
        public int PaymentCode { get; set; }
        public string CustomerName { get; set; }
        public string ReceiptNo { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string DeclarantDetails { get; set; }
        public string BranchName { get; set; }
        public string TaxDetails { get; set; }
        public string ReceivedFrom { get; set; }
        public string DeclarationNo { get; set; }
        public Decimal Amount { get; set; }
        public string AmountWords { get; set; }
        public string PaymentMode { get; set; }
        public int StatusCode { get; set; }
        public string OBRRefNo { get; set; }
        public string Remarks { get; set; }
        public string Nif { get; set; }
        public string DeclarantNo { get; set; }
        public string Period { get; set; }
    }
   
    public class TokenReportModels : BaseReportModels
    {
        public string CustomerName { get; set; }
        public string MeterNo { get; set; }
        public string TokenValue { get; set; }
        public decimal AcceptedAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal ReimbursementAmount { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int StatusCode { get; set; }
        public string AmountWords { get; set; }
        public string ReferenceNo { get; set; }
        public string ReceivedFrom { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentCode { get; set; }
        public string KWh { get; set; }
        public string ContactInfoNo { get; set; }
        public string UnitsPurchased { get; set; }
        public string AccountDebit { get; set; }
        public int Mode { get; set; }
        public decimal Vat { get; set; }
    }

    public class TopUpReportModels : BaseReportModels
    {
        public string ReceiptNo { get; set; }
        public string CustomerNo { get; set; }
        public string PaymentCode { get; set; }
        public string PhoneNo { get; set; }
        public string RetailerName { get; set; }
        public decimal Amount { get; set; }
        public string ReceivedFrom { get; set; }
        public string BankRef { get; set; }
        public string BranchName { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int StatusCode { get; set; }
        public string AmountWords { get; set; }
        public string PaymentMode { get; set; }
        public string AccountDebit { get; set; }
        public int Mode { get; set; }
    }

    public class PaywayReportModels : BaseReportModels
    {
        public string Request { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string TxId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }
        public string PaywayReceipt { get; set; }
        public string CustomerName { get; set; }
        public string Reason { get; set; }
        public string PaywayAccount { get; set; }
        public string PaywayContact { get; set; }
    }

    public class ReceiptReportModels : BaseReportModels
    {
        public int PaymentCode { get; set; }
        public dynamic CreditData { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceivedFrom { get; set; }
        public decimal Amount { get; set; }
        public string PaymentFor { get; set; }
        public string PaymentMode { get; set; }
        public string AmountWords { get; set; }
        public DateTime ReceiptDate { get; set; }   
        public string OfficeCode { get; set; }   
        public string OfficeName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DeclarantName { get; set; }
        public string DeclarantCode { get; set; }
        public string RegYear { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionRef { get; set; }
        public string PayerName { get; set; }
        public string Currency { get; set; }
        public string RegNo { get; set; }
        public string RegSerial { get; set; }
        public string AsmtSerial { get; set; }
        public string AsmtNumber { get; set; }
        public string Extra3 { get; set; }
        public string BranchName { get; set; }
        public int StatusCode { get; set; }
        public string StatusMsg { get; set; }
        public string OBRReceiptNo { get; set; }
        public int PayType { get; set; }
        public string PaymentType { get; set; }
    }

    public class ReportDataModel : BaseReportModels
    {
        public dynamic DataList { get; set; }
    }

    public class CreditSlipData : BaseReportModels
    {
        public string Office { get; set; }
        public string AssessmentYear { get; set; }
        public string AssessmentSerial { get; set; }
        public string AssessmentNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionRef { get; set; }
        public string PayerName { get; set; }
        public string Currency { get; set; }
        public string RegYear { get; set; }
        public string RegSerial { get; set; }
        public string RegNo { get; set; }
    }

    public class GeneralReportData
    {
        public string PayerName { get; set; }
        public string ReferenceNo { get; set; }
        public string BranchName { get; set; }
        public string CustomerName { get; set; }
        public string ModeName { get; set; }
        public string CommuneName { get; set; }
        public string UserName { get; set; }
        public string Mode { get; set; }
        public decimal Amount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal ReimbursementAmount { get; set; }
        public string Dr_Account { get; set; }
        public int ModeCode { get; set; }
        public DateTime PostDate { get; set; }
        public string DeclarantName { get; set; }
        public string DeclarantNo { get; set; }
        public string DR_Account { get; set; }
        public string CR_Account { get; set; }
        public DateTime PaymentDate { get; set; }
        public string RefNo { get; set; }
        public string ObrNo { get; set; }
        public string BranchCode { get; set; }
        public string ChequeNo { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentStatus { get; set; }
        public string MakerID { get; set; }
        public string CheckerID { get; set; }
        public int UserCode { get; set; }
        public string AccountCredit { get; set; }
        public string AccDebit { get; set; }
        public string Reason { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceiptDate { get; set; }
        public string DepositorName { get; set; }
        public string Checker_Id { get; set; }
        public string Status { get; set; }
        public string OfficeName { get; set; }
        public string OfficeCode { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CbsResp { get; set; }
        public string Remarks { get; set; }
        public string delcarationRef { get; set; }
        public int obrofficeCode { get; set; }
        public string PaywayAccount { get; set; }
        public string PaywayContact { get; set; }
        public string TaxName { get; set; }
        public string PaymentType { get; set; }
    }

    public class ReportGroupData
    {
        public int GroupCode { get; set; }
        public string GroupTitle { get; set; }
        public decimal GroupAmount { get; set; }

        public string paymentmode { get; set; }
        public string Request { get; set; }
        public string makerid { get; set; }
        public DateTime PaymentDate { get; set; }

        public string OfficeCode { get; set; }
        public int itemCount { get; set; }
    }

    #region New Report
    public class ReportFilterModel
    {
        [Display(Name = "Report Name")]
        public int ReportCode { get; set; }

        [Display(Name = "Date From")]
        public string DateFrom { get; set; }

        [Display(Name = "Date To")]
        public string DateTo { get; set; }

        public string Filter1 { get; set; }
        public string Filter2 { get; set; }
        public string Filter3 { get; set; }
        public string Filter4 { get; set; }
        public string Filter5 { get; set; }
        public string Filter6 { get; set; }

        public dynamic Reports { get; set; }
    }

    public class PrintReportModel
    {
        public int RespStatus { get; set; }
        public string RespMessage { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string FileName { get; set; }
        public string ReportsDir { get; set; }
        public int ParamCount { get; set; }
        public string DataSets { get; set; }
        public List<ReportData> DataList { get; set; }
        public List<ReportParams> Parameters { get; set; }

        public PrintReportModel()
        {
            DataList = new List<ReportData>();
            Parameters = new List<ReportParams>();
        }
    }

    public class ReportParams
    {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }

        public ReportParams() { }

        public ReportParams(string name, string value)
        {
            this.ParamName = name;
            this.ParamValue = value;
        }
    }

    public class ReportData
    {
        public string SourceName { get; set; }
        public string BindName { get; set; }
        public DataTable Data { get; set; }
    }

    public class ReportFilterItemModel
    {
        public string FilterName { get; set; }
        public string ValName { get; set; }
        public string ValDefault { get; set; }
        public int FilterType { get; set; }
        public int ListType { get; set; }
        public string ValPrompt { get; set; }
        public List<ListModel> ItemList { get; set; }
    }

    //public class ReportModel
    //{
    //    public int ReportCode { get; set; }

    //    [Display(Name = "Report Name")]
    //    public string ReportName { get; set; }

    //    public dynamic DataList { get; set; }
    //}
    #endregion
}
