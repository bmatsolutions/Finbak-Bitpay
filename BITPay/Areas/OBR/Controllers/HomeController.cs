using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITPay.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BITPay.Areas.OBR.Controllers
{
    [Area("obr")]
    [Authorize(Roles = "admin,guest")]
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}