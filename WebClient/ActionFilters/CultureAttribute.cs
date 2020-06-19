using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoloistWebClient.ActionFilters
{
    public class CultureAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Retreive culture from GET
            string currentCulture = filterContext.HttpContext.Request.QueryString["culture"];

            // Also, you can retreive culture from Cookie like this :
            //string currentCulture = filterContext.HttpContext.Request.Cookies["cookie"].Value;

            // Set culture
            var englishCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            System.Globalization.CultureInfo.CurrentCulture = englishCulture;
            System.Globalization.CultureInfo.CurrentUICulture = englishCulture;
        }
    }
}