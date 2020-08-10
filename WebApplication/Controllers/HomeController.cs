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
    /// <summary>
    /// Controller responsible for handling requestes on the home page,
    /// and general util services for the other controllers such as 
    /// retrieving the path for the parent directory on the file server.
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        // internal lock for preventing multithread-parallel writes to the error log file
        private static readonly object LogFileMutex = new object();


        // Home page 
        public ActionResult Index() => View();


        // About page 
        public ActionResult About() => View();


        // Documentation: user manual, design documents & diagrams, etc.
        public ActionResult Documentation() => View();


        // Source code of the application 
        public ActionResult SourceCode() => View();


        // Redirect to the github repository
        public ActionResult Repository()
        {
            string repositoryUrl = ConfigurationManager.AppSettings["repositoryUrl"];
            return Redirect(repositoryUrl);
        }


        // Contact info 
        [CrawlerFilter]
        public ActionResult Contact() => View();


        // Download the seminar 
        public FileResult Seminar()
        {
            //upload the seminar paper document into memory from the file server
            string seminarPaperPath = this.HttpContext.Server.MapPath(ConfigurationManager.AppSettings["seminarInternalPath"]);
            byte[] fileBytes = System.IO.File.ReadAllBytes(seminarPaperPath);
            string fileName = Path.GetFileName(seminarPaperPath);

            // return file content to client for downloading
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        // Privacy policy statement
        public ActionResult PrivacyPolicy() => View();


        #region GetFileServerPath
        /// <summary> 
        /// Returns the path to the parent directory on the application file server. 
        /// </summary>
        /// <param name="server"> The running web server. </param>
        /// <returns> The path to the parent directory on the application file server. /returns>
        internal static string GetFileServerPath(HttpServerUtilityBase server = null)
        {
            return server != null 
                ? server.MapPath(@"~\App_Data\")
                : AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";
        }
        #endregion


        #region WriteErrorToLog
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
                // lock the log file
                lock (LogFileMutex)
                {
                    // save the request in the log file 
                    using (StreamWriter streamWriter = System.IO.File.AppendText(logFullPath))
                    {
                        streamWriter.WriteLine(loggedError + Environment.NewLine);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}