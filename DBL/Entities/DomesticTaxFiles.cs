using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BITPay.DBL.Entities
{
    [Table("DomesticTaxFiles")]
    public class DomesticTaxFiles : BaseEntity
    {
        [NotMapped]
        public static string TableName { get { return "DomesticTaxFiles"; } }

        [Column("Id")]
        public int Id { get; set; }

        [Column("FileCode")]
        [Required()]
        public int FileCode { get; set; }

        [Column("CreateDate")]
        [Required()]
        public DateTime CreateDate { get; set; }

        [Column("TransactionCode")]
        [Required()]
        public string TransactionCode { get; set; }

        [Column("TransactionType")]
        [Required()]
        public string TransactionType { get; set; }

        [Column("UserCode")]
        [Required()]
        public int UserCode { get; set; }

        [Column("StatusCode")]
        [Required()]
        public int StatusCode { get; set; }

        [Column("Delay")]
        [Required()]
        public string Delay { get; set; }

        [Column("Amount")]
        [Required()]
        public decimal Amount { get; set; }

        [Column("CustomerName")]
        [Required()]
        [StringLength(150)]
        public string CustomerName { get; set; }

        [Column("Tin")]
        [Required()]
        public string Tin { get; set; }

        [Column("TaxPeriod")]
        [Required()]
        [StringLength(50)]
        public string TaxPeriod { get; set; }

        [Column("DeclNo")]
        [StringLength(50)]
        public string DeclNo { get; set; }

        [Column("CommuneName")]
        [StringLength(150)]
        public string CommuneName { get; set; }

        
        [Column("Extra1")]
        [StringLength(50)]
        public string Adjustment { get; set; }

        [Column("Extra2")]
        [StringLength(150)]
        public string Extra2 { get; set; }

        [Column("Extra3")]
        [StringLength(150)]
        public string Extra3 { get; set; }

        [Column("Extra4")]
        [StringLength(150)]
        public string Extra4 { get; set; }

        //[Column("NotifAttempts")]
        //[Required()]
        //public int NotifAttempts { get; set; }

        //[Column("NotifDate")]
        //public DateTime NotifDate { get; set; }

        [Column("StatusMsg")]
        [StringLength(250)]
        public string StatusMsg { get; set; }
        public string Chasis { get; internal set; }
        public string Imma { get; internal set; }
        public string CarOnwer { get; internal set; }
        public string Contracavation { get; internal set; }
        public string Document { get; internal set; }
        public string DriverName { get; internal set; }
        public string Education { get; internal set; }
        public string Infraction { get; internal set; }
        public string LicenceType { get; internal set; }
        public string Copies { get; internal set; }
        public string Vehicle { get; internal set; }
        public string Word { get; internal set; }
        public string Service { get; internal set; }
        public string TaxAdjustment { get; internal set; }
        public string Product { get; internal set; }

        ////---- View properties
        //public string StatusName { get; set; }
    }
}
