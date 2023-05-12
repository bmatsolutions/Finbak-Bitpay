using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BITPay.Models;
using Microsoft.AspNetCore.Authorization;
using BITPay.DBL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using BITPay.DBL.Models;

namespace BITPay.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "maker,checker,admin")]
    public class HomeController : BaseController
    {
        private Bl bl;
        private string logFile;
        public HomeController(IOptions<AppConfig> appSett)
        {
            bl = new Bl(appSett.Value);
            logFile = appSett.Value.LogFile;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            LogUtil.Error(logFile, "ApplicationError", "Application error occured!");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            //bl.TestSoap();
            return RedirectToAction("Dashboard");
        }
    }
}
