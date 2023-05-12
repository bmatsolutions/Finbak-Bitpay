using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BITPay.DBL.Entities
{
    [Table("MiarieTaxFiles")]
    public class MiarieTaxFile : BaseEntity
    {
        [NotMapped]
        public static string TableName { get { return "MiarieTaxFiles"; } }

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

        [Column("NoteType")]
        [Required()]
        public int NoteType { get; set; }

        [Column("NoteNo")]
        [Required()]
        [StringLength(20)]
        public string NoteNo { get; set; }

        [Column("RefNo")]
        [Required()]
        [StringLength(50)]
        public string RefNo { get; set; }

        [Column("PayerName")]
        [Required()]
        [StringLength(150)]
        public string PayerName { get; set; }

        [Column("Descr")]
        [Required()]
        [StringLength(150)]
        public string Descr { get; set; }

        [Column("Title")]
        [StringLength(250)]
        public string Title { get; set; }

        [Column("Amount")]
        [Required()]
        public decimal TaxAmount { get; set; }

        [Column("Period")]
        [Required()]
        [StringLength(20)]
        public string Period { get; set; }

        [Column("StatusMsg")]
        [StringLength(250)]
        public string StatusMsg { get; set; }

        [Column("PayPartial")]
        public int PayPartial { get; set; }

        public string TypeName { get; set; }

        //---- View properties
        public string StatusName { get; set; }
    }
}
