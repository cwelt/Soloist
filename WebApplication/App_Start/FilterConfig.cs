using System.Web.Mvc;
using CW.Soloist.WebApplication.Filters.ActionFilters;
using CW.Soloist.WebApplication.Filters.ExceptionFilters;

namespace CW.Soloist.WebApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // handler un catched exceptions 
            filters.Add(new CustomExceptionHandler());
            /* filters.Add(new HandleErrorAttribute()); */

            // configure the custom action log request filter
            filters.Add(new LogRequestFilter());

            // force an unsecured HTTP request to be re-sent over HTTPS
            filters.Add(new RequireHttpsAttribute());

            // filter automatic machine search engines traffic 
            //filters.Add(new CrawlerFilter());

            // restrict application access globaly 
            /*filters.Add(new AuthorizeAttribute());*/
        }
    }
}
