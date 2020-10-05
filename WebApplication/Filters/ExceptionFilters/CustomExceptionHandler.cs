using System.IO;
using System.Web.Mvc;

namespace CW.Soloist.WebApplication.Filters.ExceptionFilters
{
    public class CustomExceptionHandler : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            string htmlErrorPagePath = filterContext.HttpContext.Server.MapPath("/Views/Shared/Error500.html");
            
            byte[] htmlErrorPageContent = File.ReadAllBytes(htmlErrorPagePath);

            filterContext.Result = new FileContentResult(htmlErrorPageContent, "text/html");
            filterContext.ExceptionHandled = true;
        }
    }
}