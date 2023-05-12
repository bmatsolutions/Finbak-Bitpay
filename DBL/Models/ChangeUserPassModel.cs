using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BITPay.DBL.Models
{
    public class ChangeUserPassModel
    {
        [Required]
        public int UserCode { get; set; }

        [Required]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 5)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Re-type Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do no match!")]
        public string ConfirmPassword { get; set; }
    }
}
