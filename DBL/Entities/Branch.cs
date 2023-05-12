using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BITPay.DBL.Entities
{
    [Table("Branches")]
    public class Branch
    {
        [NotMapped]
        public static string TableName { get { return "Branches"; } }

        [Column("Id")]
        [Required()]
        public int Id { get; set; }

        [Column("BranchCode")]
        [Required()]
        public int BranchCode { get; set; }

        [Column("BranchName")]
        [Required()]
        [StringLength(35)]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Column("CBS_Code")]
        [Required()]
        [StringLength(10)]
        [Display(Name = "CBS Code")]
        public string CBS_Code { get; set; }

        [Column("Cash_GL_Account")]
        [Required()]
        [StringLength(20)]
        [Display(Name = "Cash GL Account")]
        public string Cash_GL_Account { get; set; }

        [Column("BranchStat")]
        [Required()]
        public int BranchStat { get; set; }
    }
}
