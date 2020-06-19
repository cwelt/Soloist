using SoloistWebClient.ActionFilters;
using System.Web;
using System.Web.Mvc;

namespace SoloistWebClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CultureAttribute());
        }
    }
}
