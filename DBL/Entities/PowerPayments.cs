using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BITPay.DBL.Entities
{
    [Table("PowerPayments")]
    public class PowerPayments : BaseEntity
    {
        [NotMapped]
        public static string TableName { get { return "PowerPayments"; } }

        [Column("Id")]
        public int Id { get; set; }

        [Column("PaymentCode")]
        [Required()]
        public int PaymentCode { get; set; }

        [Column("FileCode")]
        [Required()]
        public string FileCode { get; set; }

        [Column("ReceiptNo")]
        [Required()]
        [StringLength(20)]
        public string ReceiptNo { get; set; }

        [Column("CreateDate")]
        [Required()]
        public DateTime CreateDate { get; set; }

        [Column("UserCode")]
        [Required()]
        public int UserCode { get; set; }

        [Column("Amount")]
        [Required()]
        public decimal Amount { get; set; }

        [Column("StatusCode")]
        [Required()]
        public int StatusCode { get; set; }

        [Column("ModeCode")]
        [Required()]
        public int ModeCode { get; set; }

        [Column("Cr_Account")]
        [StringLength(20)]
        public string Cr_Account { get; set; }

        [Column("Dr_Account")]
        [StringLength(20)]
        public string Dr_Account { get; set; }

        [Column("Remarks")]
        [StringLength(150)]
        public string Remarks { get; set; }

        [Column("PostAttempts")]
        [Required()]
        public int PostAttempts { get; set; }

        [Column("PostDate")]
        public DateTime PostDate { get; set; }

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

        [Column("StatusMsg")]
        [StringLength(250)]
        public string StatusMsg { get; set; }

        [Column("ApplyCharge")]
        public bool ApplyCharge { get; set; }

        [Column("PhoneNo")]
        [StringLength(20)]
        public string PhoneNo { get; set; }

        [Column("CustomerNo")]
        [StringLength(50)]
        public string CustomerNo { get; set; }

        [Column("AccountName")]
        [StringLength(150)]
        public string AccountName { get; set; }
    }
}
