using CW.Soloist.WebApplication.Filters.ActionFilters;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
            return View();
        }

        public ActionResult Documentation()
        {
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

        public FileResult Seminar()
        {
            // save file and return it for client to download

            string seminarPaperPath = this.HttpContext.Server.MapPath("/App_Data/Seminar/GA_Seminar.pdf");

            byte[] fileBytes = System.IO.File.ReadAllBytes(seminarPaperPath);
            string fileName = Path.GetFileName(seminarPaperPath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        internal static string GetFileServerPath(HttpServerUtilityBase server = null)
        {
            if (server != null )
                return server.MapPath(@"~\App_Data\");
            else return AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";
        }
    }
}