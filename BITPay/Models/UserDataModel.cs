using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BITPay.Models
{
    public class UserDataModel
    {
        public int UserId { get; set; }
        public string FullNames { get; set; }
        public string UserName { get; set; }
        public int UserRole { get; set; }
        public string SessionNo { get; set; }
        public int UserCode { get; set; }
        public int UserStatus { get; set; }
        public string Title { get; set; }
        public string BankName { get; internal set; }
        public int Post { get; set; }
        // public string brachCode { get; set; }
    }
}