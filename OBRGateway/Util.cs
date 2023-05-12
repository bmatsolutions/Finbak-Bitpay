using OBRClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OBRClient
{
    public class Util
    {
        public static void LogError(string logFile, string module, Exception ex, bool isError = true)
        {
            try
            {
                if (string.IsNullOrEmpty(logFile))
                    return;

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
    }
}