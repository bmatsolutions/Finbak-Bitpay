using OBRClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OBRGateway
{
    public class Util
    {
        public static void LogError(string module, Exception ex, bool isError = true)
        {
            try
            {
                //string logDir =  Path.Combine(Directory.GetCurrentDirectory(), "logs");
                string logDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/");

                //---- Create Directory if it does not exist              
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                string logFile = Path.Combine(logDir, "ErrorLog.log");
                //--- Delete log if it more than 500Kb
                if (File.Exists(logFile))
                {
                    FileInfo fi = new FileInfo(logFile);
                    if ((fi.Length / 1000) > 500)
                        fi.Delete();
                }
                //--- Create stream writter
                StreamWriter stream = new StreamWriter(logFile, true);
                stream.WriteLine(string.Format("{0}|{1:dd-MMM-yyyy HH:mm:ss}|{2}|{3}",
                    isError ? "ERROR" : "INFOR",
                    DateTime.Now,
                    module,
                    isError ? ex.ToString() : ex.Message));
                stream.Close();
            }
            catch (Exception) { }
        }

        public static string ReadLog()
        {
            string lodData = "No log data found!";
            try
            {
                string logDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/");
                string logFile = Path.Combine(logDir, "ErrorLog.log");
                //--- Delete log if it more than 500Kb
                if (File.Exists(logFile))
                {
                    lodData = File.ReadAllText(logFile);
                }               
            }
            catch (Exception) { }

            return lodData;
        }

        public static string GetLogFile()
        {
            string logDir = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/");
            return Path.Combine(logDir, "ErrorLog.log");
        }

        public static RequestResponseModel HandleException(string module, Exception ex)
        {
            Util.LogError(module, ex);
            return new RequestResponseModel
            {
                Status = 1,
                Message = "An error occured in the request"
            };
        }
    }
}