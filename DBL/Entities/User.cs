using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BITPay.DBL.Entities
{
    [Table("Users")]
    public class User
    {
        [NotMapped]
        public string TableName { get { return "Users"; } }

        [Column("Id")]
        public int Id { get; set; }

        [Column("UserCode")]
        public int UserCode { get; set; }

        [Column("UserName")]
        [Required()]
        [StringLength(20)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Column("FullNames")]
        [Required()]
        [StringLength(35)]
        [Display(Name = "Full Names")]
        public string FullNames { get; set; }

        [Column("Email")]
        [Required()]
        [StringLength(50)]
        [Display(Name = "E-Mail Address")]
        public string Email { get; set; }

        [Column("Pwd")]
        [Required()]
        [StringLength(20)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Pwd { get; set; }

        [Column("Salt")]
        [StringLength(250)]
        public string Salt { get; set; }

        [Column("UserStatus")]
        public int UserStatus { get; set; }

        [Column("UserRole")]
        [Required()]
        [Display(Name = "User Role")]
        public int UserRole { get; set; }

        [Column("BranchCode")]
        [Required()]
        [Display(Name = "User Branch")]
        public int BranchCode { get; set; }

        [Column("LastLogin")]
        public DateTime LastLogin { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("ChangePwd")]
        public bool ChangePwd { get; set; }

        [Column("Attempts")]
        public int Attempts { get; set; }

        //----- For new user creation validation
        [Required]
        [Display(Name = "Re-type Password")]
        [DataType(DataType.Password)]
        [Compare("Pwd", ErrorMessage = "Passwords do no match!")]
        public string ConfirmPassword { get; set; }        
    }
}
