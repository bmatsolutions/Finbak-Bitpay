using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BITPay.DBL.Entities
{
    [Table("PaymentModes")]
    public class PaymentMode
    {
        [NotMapped]
        public static string TableName { get { return "PaymentModes"; } }

        [Column("Id")]
        public int Id { get; set; }

        [Column("ModeCode")]
        [Required()]
        public int ModeCode { get; set; }

        [Column("ModeName")]
        [Required()]
        [StringLength(30)]
        public string ModeName { get; set; }

        [Column("Approval")]
        [Required()]
        public bool Approval { get; set; }

        [Column("GL_Account")]
        [StringLength(20)]
        public string GL_Account { get; set; }

        [Column("Charge")]
        [Required()]
        public decimal Charge { get; set; }

        [Column("CBS_Txn_Code")]
        [StringLength(10)]
        public string CBS_Txn_Code { get; set; }
    }
}
