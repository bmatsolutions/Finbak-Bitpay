using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public class CreateFileResultModel
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public byte[] FileData { get; set; }
        public string ContentType { get; set; }
        public string DownloadName { get; set; }
    }
}
