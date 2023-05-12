using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BITPay.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using UAParser;

namespace BITPay.Controllers
{
    public class BaseController : Controller
    {
        public UserDataModel SessionUserData
        {
            get
            {
                {
                    UserDataModel userDataSerializeModel = null;
                    if (base.User is ClaimsPrincipal)
                    {
                        string claim = BaseController.GetClaim((base.User as ClaimsPrincipal).Claims.ToList<Claim>(), "userData");
                        if (!string.IsNullOrEmpty(claim))
                        {
                            userDataSerializeModel = JsonConvert.DeserializeObject<UserDataModel>(claim);
                        }
                    }

                    base.ViewData["UserData"] = (userDataSerializeModel ?? new UserDataModel());
                    if (userDataSerializeModel == null)
                    {
                        //string url = base.Url.Action("Login", "Account");
                        //requestContext.HttpContext.Response.Redirect(url);
                    }

                    return userDataSerializeModel;
                }
            }
        }

        public string GetIP()
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            string ip = remoteIpAddress.ToString();
            return ip;
        }
         
        public string GetUserBrowser()
        {
            
            var userAgent = Request.Headers["User-Agent"];
            //UserAgent.UserAgent ua = new UserAgent.UserAgent(userAgent);
            string uaString = Convert.ToString(userAgent[0]);
            Parser uaParser = Parser.GetDefault();
            UAParser.ClientInfo c = uaParser.Parse(uaString);
            string browserdata = c.ToString();

            return browserdata;
        }

       

        public static string GetClaim(List<Claim> claims, string key)
        {
            Claim claim = claims.FirstOrDefault((Claim c) => c.Type == key);
            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }

        public void Success(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Success, message, dismissable, "fa fa-check");
        }

        public void Information(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Information, message, dismissable, "fa fa-info-circle");
        }

        public void Warning(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Warning, message, dismissable, "fa fa-warning");
        }

        public void Danger(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Danger, message, dismissable, "fa fa-times-circle");
        }

        private void AddAlert(string alertStyle, string message, bool dismissable, string iconClass)
        {
            var alerts = new List<Alert>();

            string jsonData = TempData.ContainsKey(Alert.TempDataKey) ? (string)TempData[Alert.TempDataKey] : "";
            if (!string.IsNullOrEmpty(jsonData))
                alerts = JsonConvert.DeserializeObject<List<Alert>>(jsonData);

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable,
                IconClass = iconClass
            });

            TempData[Alert.TempDataKey] = JsonConvert.SerializeObject(alerts);
        }
    }
}