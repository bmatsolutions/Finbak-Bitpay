using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITPay.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BITPay.Areas.ReportViewer.Controllers
{
    [Area("ReportViewer")]
    [Authorize(Roles = "admin,reportviewer")]
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}