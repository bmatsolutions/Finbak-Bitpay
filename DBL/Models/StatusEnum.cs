using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL.Models
{
    public enum StatusEnum
    {
        OK = 200,
        AUTH_ERROR = 300,
        VALIDATION_ERROR = 400,
        SERVER_ERROR = 500
    }
}
