using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FileServer()
        {
            string fileServerAppDataUrl = ConfigurationManager.AppSettings["FileServerAppDataUrl"];
            return Redirect(fileServerAppDataUrl);
        }
    }
}