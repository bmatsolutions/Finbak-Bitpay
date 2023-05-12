using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OBRGateway.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "OBR Gateway";
            return View();
        }

        [HttpGet]
        public ActionResult Logs()
        {
            string logData = Util.ReadLog();
            return Content(logData, "text/plain");
        }
    
        [HttpGet]
        public ActionResult Test()
        {
            try
            {
                string amountStr = "3621784.0";
                var amount = Convert.ToDecimal(amountStr);
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Index");
        }
    }
}
