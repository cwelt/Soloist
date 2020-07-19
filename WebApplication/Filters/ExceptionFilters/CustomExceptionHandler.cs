using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CW.Soloist.WebApplication.Filters.ExceptionFilters
{
    public class CustomExceptionHandler : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var path = filterContext.HttpContext.Server.MapPath("/Views/Shared/Error500.html");
            
            var bytes = File.ReadAllBytes(path);

            filterContext.Result = new FileContentResult(bytes, "text/html");
            filterContext.ExceptionHandled = true;
        }
    }
}