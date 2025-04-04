﻿using System.Web.Mvc;

namespace CW.Soloist.WebApplication.Filters.ActionFilters
{
    /// <summary>
    /// Filter for blocking access to requests that come from 
    /// web browser crawlers (spiders) such as search engines. 
    /// </summary>
    public class CrawlerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.HttpContext.Request.Browser.Crawler)
            {
                filterContext.Result = new HttpNotFoundResult();
            }
        }
    }
}