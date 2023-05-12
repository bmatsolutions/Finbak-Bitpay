using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class PostResultModel
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public bool AllowRetry { get; set; }
        public string CBSRefNo { get; set; }
    }
}
