using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class MairieBaseResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("resp_msg")]
        public string RespMessage { get; set; }
    }

    public class MairieTaxNotePaymentModel
    {
        [JsonProperty("typ")]
        public int TaxType { get; set; }

        [JsonProperty("amt")]
        public decimal Amount { get; set; }

        [JsonProperty("agt_ref")]
        public string AgentRef { get; set; }

        [JsonProperty("ref_no")]
        public string RefNo { get; set; }

        [JsonProperty("dscr")]
        public string Desciption { get; set; }
    }

    public class MairieTaxPayResultModel: MairieBaseResponse
    {
        [JsonProperty("ref_no")]
        public string RefNo { get; set; }

        [JsonProperty("amt_bal")]
        public decimal AmountBal { get; set; }

        [JsonProperty("amt_paid")]
        public decimal AmountPaid { get; set; }

        [JsonProperty("amt_excess")]
        public decimal ExcessAmount { get; set; }

        [JsonProperty("descr")]
        public string Description { get; set; }

        [JsonProperty("amt")]
        public string Amount { get; set; }

        [JsonProperty("agt_ref")]
        public string BankRef { get; set; }

        [JsonProperty("hdg")]
        public string Heading { get; set; }

        [JsonProperty("typ")]
        public string Type { get; set; }

        [JsonProperty("prd")]
        public string Period { get; set; }
    }

    public class MairieTaxpayerQueryModel
    {
        [JsonProperty("tid")]
        [Display(Name = "Taxpayer No")]
        [Required]
        public string TaxpayerId { get; set; }
    }

    public class MairieTaxpayerModel: MairieBaseResponse
    {
        [JsonProperty("tid")]
        public string TaxpayerId { get; set; }

        [JsonProperty("reg_no")]
        public string RegNo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("pno")]
        public string PhoneNo { get; set; }
    }

    public class QueryResp
    {
        [JsonProperty("stat")]
        public int Stat { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public MairieTaxNoteDataModel Content { get; set; }
        
    }

    public class MairieTaxNoteDataModel: MairieBaseResponse
    {
        [JsonProperty("ttl_amt")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("ttl_amt_src")]
        public decimal TotalAmountSource { get; set; }

        [JsonProperty("tax_notes")]
        public MairieTaxNoteModel[] TaxNotes { get; set; }

        [JsonProperty("tax_typ")]
        public int TaxType { get; set; }
    }

    public class MairieTaxNoteModel
    {
        [JsonProperty("nid")]
        public int NoteId { get; set; }

        [JsonProperty("ref_no")]
        public string RefNo { get; set; }

        [JsonProperty("ttl")]
        public string Title { get; set; }

        [JsonProperty("tp_name")]
        public string TaxPayerName { get; set; }

        [JsonProperty("nic")]
        public string Nic { get; set; }

        [JsonProperty("tn_dsc")]
        public string TaxNoteDescr { get; set; }

        [JsonProperty("card_no")]
        public string CardNo { get; set; }

        [JsonProperty("card")]
        public string Card { get; set; }

        [JsonProperty("card_lb")]
        public string CardLabel { get; set; }

        [JsonProperty("tax_amt")]
        public decimal TaxAmount { get; set; }

        [JsonProperty("pnt_no")]
        public string PrintNo { get; set; }

        [JsonProperty("prd")]
        public string Period { get; set; }

        [JsonProperty("nxt_prd")]
        public string NextPeriod { get; set; }

        [JsonProperty("amt")]
        public decimal Amount { get; set; }

        [JsonProperty("amt_paid")]
        public decimal TaxAmountPaid { get; set; }

        [JsonProperty("paid")]
        public bool IsPaid { get; set; }

        [JsonProperty("late")]
        public bool IsLate { get; set; }

        [JsonProperty("tn_typ")]
        public int TaxNoteType { get; set; }

        [JsonProperty("typ_name")]
        public string NoteTypeName { get; set; }
    }

    public class MiariePayement
    {
        [JsonProperty("tn_type")]
        public int TaxNoteType { get; set; }
        [JsonProperty("amt")]
        public decimal Amount { get; set; }
        [JsonProperty("src_amt")]
        public decimal SrcAmount { get; set; }
        [JsonProperty("agt_ref")]
        public string AgentRef { get; set; }
        [JsonProperty("ref_no")]
        public string RefNo { get; set; }
        [JsonProperty("prd")]
        public string Period { get; set; }
        [JsonProperty("dscr")]
        public string Descr { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class MairieTaxPaymentResult
    {
        [JsonProperty("stat")]
        public int Status { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public MairieTaxNotePayResultModel Content { get; set; }
    }

    public class MairieTaxNotePayResultModel
    {
        [JsonProperty("success")]
        public bool Successful { get; set; }

        [JsonProperty("err_msg")]
        public string ErrorMessage { get; set; }

        [JsonProperty("ref_no")]
        public string RefNo { get; set; }

        [JsonProperty("pay_ref")]
        public string PaymentRef { get; set; }


        [JsonProperty("dets")]
        public MairieTNPaymentDetails Details { get; set; }
    }

    public class MairieTNPaymentDetails
    {

        [JsonProperty("src_amt")]
        public decimal SourceAmount { get; set; }

        [JsonProperty("amt")]
        public decimal Amount { get; set; }

        [JsonProperty("dsc")]
        public string Description { get; set; }

        [JsonProperty("prd")]
        public string Period { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("bk_ref")]
        public string BankRef { get; set; }

        [JsonProperty("agt_ref")]
        public string AgentRef { get; set; }

        [JsonProperty("bk_name")]
        public string BankName { get; set; }

        [JsonProperty("bk_user")]
        public int BankUser { get; set; }

        [JsonProperty("bk_dsc")]
        public string BankDescr { get; set; }

        [JsonProperty("note_amt")]
        public decimal NoteAmount { get; set; }

        [JsonProperty("dt")]
        public DateTime PaidDate { get; set; }
    }

    public class MairieQueryTaxTypeDataResult: MairieBaseResponse
    {
        [JsonProperty("types")]
        public MairieListItemModel[] Types { get; set; }

        [JsonProperty("fields")]
        public MarieDataFieldModel[] Fields { get; set; }
    }

    public class MairieListItemModel
    {
        [JsonProperty("code")]
        public string ItemCode { get; set; }

        [JsonProperty("name")]
        public string ItemName { get; set; }
    }

    public class MarieDataFieldModel
    {
        [JsonProperty("typ_code")]
        public int TypeCode { get; set; }

        [JsonProperty("fld_name")]
        public string FieldName { get; set; }

        [JsonProperty("fld_ttl")]
        public string FieldTitle { get; set; }

        [JsonProperty("is_req")]
        public bool IsRequired { get; set; }

        [JsonProperty("dt_typ")]
        public int DataType { get; set; }


        [JsonProperty("list")]
        public MairieListItemModel[] ListItems { get; set; }
    }
}
