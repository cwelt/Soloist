using CW.Soloist.WebApplication.ActionFilters;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoloistWebClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ValidateInput(false)]
        public ActionResult SourceCode()
        {
            return View();
            //return Content("<script>window.open('{https://github.com/cwelt/Soloist}','_blank')</script>");
        }

        public ActionResult Repository()
        {
            string repositoryUrl = ConfigurationManager.AppSettings["repositoryUrl"];
            return Redirect(repositoryUrl);
        }

        [CrawlerFilter]
        public ActionResult Contact()
        {
            ViewBag.Message = "Feel free to contact me in one of the following medium channels.";

            return View();
        }
    }
}