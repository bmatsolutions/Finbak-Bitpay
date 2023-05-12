using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class RetailerTopUp
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AppID { get; set; }
        public string AppToken { get; set; }
        public string ProviderId { get; set; }
        public string Amount { get; set; }
        public string ContactInfo { get; set; }
        public string CustomerNumber { get; set; }
        public string TxId { get; set; }
    }

}
