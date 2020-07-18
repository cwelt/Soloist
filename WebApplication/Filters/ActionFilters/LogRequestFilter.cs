using Newtonsoft.Json;
using SoloistWebClient.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication.Filters.ActionFilters
{
    /// <summary>
    /// Action filter responsible for logging all the HTTP requests 
    /// that are send to the web server. 
    /// <para> Only the primary data is kept </para>
    /// </summary>
    public class LogRequestFilter : ActionFilterAttribute
    {
        // retrieve the actual physical path of the file server 
        private string fileServerPath = HomeController.GetFileServerPath();

        /// <summary>
        /// Intervenes before the Http request executes and logs the 
        /// relevant data to disk.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // get the actual data from the given context 
            var logRecord = new
            {
                // Host user machine and user properties 
                HostIP = filterContext.HttpContext.Request.UserHostAddress,
                HostName = filterContext.HttpContext.Request.UserHostName,
                Browser = filterContext.HttpContext.Request.Browser.Browser,

                // Http properties 
                HttpMethod = filterContext.HttpContext.Request.HttpMethod,
                HttpHeaders = filterContext.HttpContext.Request.Headers,
                ContentType = filterContext.HttpContext.Request.ContentType,
                RequestLength = filterContext.HttpContext.Request.InputStream.Length,
                Cookies = filterContext.HttpContext.Request.Cookies,
                //RequestParams = filterContext.HttpContext.Request.Params,
                //Form = filterContext.HttpContext.Request.Form,

                // MVC properties 
                Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Action = filterContext.ActionDescriptor.ActionName,
                //ActionParameters = filterContext.ActionParameters.Keys,

                // Timestamp 
                DateTime = filterContext.HttpContext.Timestamp.ToString(format: "dddd, MMMM dd, yyyy h:mm:ss tt")
            };

            // serialize the data as a JSON object 
            string loggedAction = JsonConvert.SerializeObject(logRecord);
            
            // set full path for the log file 
            string logInternalPath = ConfigurationManager.AppSettings["requestLogInternalPath"];
            string logFullPath = fileServerPath + logInternalPath;

            // save the request in the log file 
            using (StreamWriter streamWriter = File.AppendText(logFullPath))
            {
                streamWriter.WriteLine(loggedAction + Environment.NewLine);
            }
        }
    }
}