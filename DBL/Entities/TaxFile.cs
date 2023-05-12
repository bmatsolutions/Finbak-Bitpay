using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BITPay.DBL.Entities
{
    [Table("TaxFiles")]
    public class TaxFile : BaseEntity
    {
        [NotMapped]
        public static string TableName { get { return "TaxFiles"; } }

        [Column("PayType")]
        [Required()]
        public int PayType { get; set; }

        [Column("Id")]
        public int Id { get; set; }

        [Column("FileCode")]
        [Required()]
        public int FileCode { get; set; }

        [Column("CreateDate")]
        [Required()]
        public DateTime CreateDate { get; set; }

        [Column("UserCode")]
        [Required()]
        public int UserCode { get; set; }

        [Column("StatusCode")]
        [Required()]
        public int StatusCode { get; set; }

        [Column("TaxAmount")]
        [Required()]
        public decimal TaxAmount { get; set; }

        [Column("OfficeCode")]
        [Required()]
        [StringLength(20)]
        public string OfficeCode { get; set; }

        [Column("OfficeName")]
        [Required()]
        [StringLength(50)]
        public string OfficeName { get; set; }

        [Column("DclntCode")]
        [Required()]
        [StringLength(20)]
        public string DclntCode { get; set; }

        [Column("DclntName")]
        [Required()]
        [StringLength(50)]
        public string DclntName { get; set; }

        [Column("CompanyCode")]
        [Required()]
        [StringLength(20)]
        public string CompanyCode { get; set; }

        [Column("CompanyName")]
        [Required()]
        [StringLength(150)]
        public string CompanyName { get; set; }

        [Column("AccountHolder")]
        [Required()]
        [StringLength(150)]
        public string AccountHolder { get; set; }

        [Column("RegYear")]
        [Required()]
        public int RegYear { get; set; }

        [Column("AsmtSerial")]
        [Required()]
        [StringLength(20)]
        public string AsmtSerial { get; set; }

        [Column("AsmtNumber")]
        [Required()]
        [StringLength(20)]
        public string AsmtNumber { get; set; }

        [Column("RegSerial")]
        [Required()]
        [StringLength(20)]
        public string RegSerial { get; set; }

        [Column("RegNumber")]
        [Required()]
        [StringLength(20)]
        public string RegNumber { get; set; }

        [Column("Extra1")]
        [StringLength(150)]
        public string Extra1 { get; set; }

        [Column("Extra2")]
        [StringLength(150)]
        public string Extra2 { get; set; }

        [Column("Extra3")]
        [StringLength(150)]
        public string Extra3 { get; set; }

        [Column("Extra4")]
        [StringLength(150)]
        public string Extra4 { get; set; }

        [Column("NotifAttempts")]
        [Required()]
        public int NotifAttempts { get; set; }

        [Column("NotifDate")]
        public DateTime NotifDate { get; set; }

        [Column("StatusMsg")]
        [StringLength(250)]
        public string StatusMsg { get; set; }

        //---- View properties
        public string StatusName { get; set; }
        public string TaxPayerName { get; internal set; }
        public string TransactionCode { get; internal set; }
        public string TransactionRef { get; internal set; }
        public string Currency { get; internal set; }
    }
}
