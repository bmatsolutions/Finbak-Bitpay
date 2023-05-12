using BITPay.DBL.Models;
using BITPay.Models;
using Bmat.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BITPay
{
    public class Util
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static AppConfig GetAppConfig(IConfiguration config, IWebHostEnvironment env)
        {
            var appConfig = new AppConfig();
            appConfig.DatabaseType = 0;// Convert.ToInt32(config.GetSection("DbConnData:DbType").Value);
            if (appConfig.DatabaseType == 0)
            {
                string connId = config.GetSection("DbConnData:Id").Value;
                string connKey = config.GetSection("DbConnData:Key").Value;
                string connData = config.GetSection("DbConnData:Data").Value;

                ConnectionManager conMan = new ConnectionManager();
                var cData = conMan.GetConnectionString(connData, connId, connKey);

                appConfig.ConnectionString = cData.ConnectionString;
            }

            appConfig.LogFile = GetErrorLogFile(env);

            return appConfig;
        }

        public static string GetErrorLogFile(IWebHostEnvironment env)
        {
            try
            {
                string logDir = Path.Combine(env.ContentRootPath, "logs");

                //---- Create Directory if it does not exist              
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                return Path.Combine(logDir, "ErrorLog.log");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetImagesRepository()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            return Configuration["AppSet:ImgsRep"];
        }

        public static string GetReportsDir()
        {
            try
            {
                string repDir = Path.Combine(Directory.GetCurrentDirectory(), "reports");
                //---- Create Directory if it does not exist              
                if (!Directory.Exists(repDir))
                {
                    Directory.CreateDirectory(repDir);
                }
                return repDir;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string UnixTimeStamp()
        {
            DateTime start = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - start;
            return Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
        }

        public static void ClearSimilarImages(string dir, string fileId)
        {
            try
            {
                int index = fileId.IndexOf("_");
                if (index > 1)
                {
                    string filter = fileId.Substring(0, index + 1) + "*";
                    DirectoryInfo dInfo = new DirectoryInfo(dir);
                    var files = dInfo.GetFiles(filter);
                    foreach (var file in files)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ) { }
        }

        public static UserDataModel GetCurrentUserData(IEnumerable<ClaimsIdentity> claims)
        {
            string userData = claims.First(u => u.IsAuthenticated && u.HasClaim(c => c.Type == "userData")).FindFirst("userData").Value;
            if (string.IsNullOrEmpty(userData))
                return null;

            return JsonConvert.DeserializeObject<UserDataModel>(userData);
        }
        
    }

    public class Alert
    {
        public const string TempDataKey = "TempDataAlerts";
        public string AlertStyle { get; set; }
        public string Message { get; set; }
        public bool Dismissable { get; set; }
        public string IconClass { get; set; }
    }

    public static class AlertStyles
    {
        public const string Success = "success";
        public const string Information = "info";
        public const string Warning = "warning";
        public const string Danger = "danger";
    }
}
