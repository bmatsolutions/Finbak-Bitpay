using BITPay.DBL.Enums;
using System.ComponentModel.DataAnnotations;

namespace BITPay.DBL.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int UserCode { get; set; }
        public string FullNames { get; set; }
        public string PhoneNo { get; set; }
        public int UserRole { get; set; }
        public string UserRoleName { get; set; }
        public UserLoginStatus UserStatus { get; set; }
        public int RespStatus { get; set; }
        public string RespMessage { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
       // public string brachCode { get; set; }
    }
    public class vwUser
    {
        [Required]
        public int UserCode { get; set; }

        [Required]
        [Display(Name ="Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Full Names")]
        public string FullNames { get; set; }

        [Display(Name = "E-Mail Address")]
        public string Email { get; set; }

        [Display(Name = "User Role")]
        public int UserRole { get; set; }

        [Display(Name = "Branch")]
        public int BranchCode { get; set; }
        public string Maker { get; set; }

    }

    public class vwSystemSett
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
        public string Descr { get; set; }
       // public string Maker { get; set; }

    }
}
