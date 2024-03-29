﻿using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using CW.Soloist.WebApplication.Controllers;


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
        private static readonly string FileServerPath = HomeController.GetFileServerPath();

        // internal lock for prevent multithread parallel writes to the log file
        private static readonly object LogFileMutex = new object();

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

                // MVC properties 
                Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Action = filterContext.ActionDescriptor.ActionName,

                // Timestamp 
                DateTime = filterContext.HttpContext.Timestamp.ToString(format: "dddd, MMMM dd, yyyy h:mm:ss tt")
            };

            // serialize the data as a JSON object 
            string loggedAction = JsonConvert.SerializeObject(logRecord);
            
            // set full path for the log file 
            string logInternalPath = ConfigurationManager.AppSettings["requestLogInternalPath"];
            string logFullPath = FileServerPath + logInternalPath;

            // lock the log file
            lock (LogFileMutex)
            {
                // save the request in the log file 
                using (StreamWriter streamWriter = File.AppendText(logFullPath))
                {
                    streamWriter.WriteLine(loggedAction + Environment.NewLine);
                }
            }
        }
    }
}