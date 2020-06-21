using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication.Filters.ActionFilters
{
    public class LogRequestFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var logRecord = new
            {
                Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Action = filterContext.ActionDescriptor.ActionName,
                IP = filterContext.HttpContext.Request.UserHostAddress,
                Browser = filterContext.HttpContext.Request.Browser.Browser,
                ServerMachine = filterContext.HttpContext.Server.MachineName,
                ContentType = filterContext.HttpContext.Request.ContentType,
                DateTime = filterContext.HttpContext.Timestamp
            };

            string loggedAction = JsonConvert.SerializeObject(logRecord);
            Debug.WriteLine(loggedAction);
            

            //string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\;

/*            using (StreamWriter streamWriter = File.AppendText("SoloistWebSiteRequestLog.txt"))
            {
                streamWriter.WriteLine(loggedAction);
            }*/
        }
    }
}