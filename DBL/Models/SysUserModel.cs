using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class SysUserModel
    {
        public int UserCode { get; set; }
        public string UserName { get; set; }
        public string FullNames { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLogin { get; set; }
        public int UserStatus { get; set; }
        public int UserRole { get; set; }
        public string UserStatusName { get; set; }
        public string UserRoleName { get; set; }
        public string Cash_Account { get; set; }
        public string BranchName { get; set; }
    }

    public class Syssetting
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
        public string Descr { get; set; }
        public string Status { get; set; }
    }

    public class AuditModel
    {
        public int UserCode { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public DateTime AuditDate { get; set; }
        public string ActionDescription { get; set; }
        public string IPAddress { get; set; }
        public string WindowBrowser { get; set; }
    }
}
