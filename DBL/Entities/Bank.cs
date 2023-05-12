using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BITPay.DBL.Entities
{
    [Table("Banks")]
    public class Bank
    {
        [NotMapped]
        public static string TableName { get { return "Banks"; } }

        [Column("Id")]
        [Required()]
        public int Id { get; set; }

        [Column("BankCode")]
        [Required()]
        public int BankCode { get; set; }

        [Column("BankID")]
        [Required()]
        [Display(Name = "Bank Code")]
        public string BankID { get; set; }

        [Column("BankName")]
        [Required()]
        [StringLength(35)]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [NotMapped]
        public int mode { get; set; }
    }
}
