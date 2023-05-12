using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class ReportGenModel
    {
        public int ReportCode { get; set; }
        public string dateto { get; set; }
        public string datefrom { get; set; }
        public int user { get; set; }
        public int branch { get; set; }
        public string office { get; set; }
        public int ReportType { get; set; }
    }
}
