using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BITPayService.Models
{
   public class MairieReportDataItem
    {
        public string BankRef { get; set; }
        public decimal Amount { get; set; }
        public string RefNo { get; set; }
        public int ItemType { get; set; }
    }
}
