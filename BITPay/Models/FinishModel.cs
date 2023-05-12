using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BITPay.Models
{
    public class FinishModel
    {
        public int Code { get; set; }
        public int PaymentCode { get; set; }
        public string Message { get; set; }
        public string ApprovalMessage { get; set; }
        public string OBRMsg { get; set; }
        public int AutoPrint { get; set; }
        public bool NeedApproval { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public int TaxType { get; set; }
    }
}
