using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class UserProfileModel: ChangeUserPassModel
    {
        public string UserName { get; set; }       
        public string FullNames { get; set; }
        public string EMail { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserStatus { get; set; }
        public int UserRole { get; set; }
    }
}
