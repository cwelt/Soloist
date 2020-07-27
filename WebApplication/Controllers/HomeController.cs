using CW.Soloist.WebApplication.Filters.ActionFilters;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication.Controllers
{
    [AllowAnonymous]
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
            if (server != null)
                return server.MapPath(@"~\App_Data\");
            else return AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        /// <summary>
        /// Writes the given error message with to an error log on the server, alongside 
        /// other properties from given http context such as the logged-in user & http headers.
        /// </summary>
        /// <param name="context"> The Http context of the sbuject request. </param>
        /// <param name="errorMessage"> The error message that is to be logged. </param>
        internal static void WriteErrorToLog(HttpContextBase context, string errorMessage)
        {
            // get the actual data from the given context 
            var logRecord = new
            {
                // Host user machine and user properties 
                User = context.User.Identity.Name,
                HostIP = context.Request.UserHostAddress,
                HostName = context.Request.UserHostName,

                // add the error message 
                ErrorMessage = errorMessage,

                // Http properties 
                HttpMethod = context.Request.HttpMethod,
                HttpHeaders = context.Request.Headers,
                ContentType = context.Request.ContentType,
                RequestLength = context.Request.InputStream.Length,
                Cookies = context.Request.Cookies,
                Url = context.Request.Url,

                // Timestamp 
                DateTime = context.Timestamp.ToString(format: "dddd, MMMM dd, yyyy h:mm:ss tt")
            };

            // serialize the data as a JSON object 
            string loggedError = JsonConvert.SerializeObject(logRecord);

            // set full path for the log file 
            string logInternalPath = ConfigurationManager.AppSettings["errorLogInternalPath"];
            string logFullPath = GetFileServerPath() + logInternalPath;

            try
            {
                // save the request in the log file 
                using (StreamWriter streamWriter = System.IO.File.AppendText(logFullPath))
                {
                    streamWriter.WriteLine(loggedError + Environment.NewLine);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}