﻿using CW.Soloist.WebApplication.Filters.ActionFilters;
using CW.Soloist.WebApplication.Filters.ExceptionFilters;
using System.Web;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute()); 
            //filters.Add(new CustomExceptionHandler());

            // configure the custom action log request filter
            filters.Add(new LogRequestFilter());

            // restrict application access globaly 
            filters.Add(new AuthorizeAttribute());

        }
    }
}
