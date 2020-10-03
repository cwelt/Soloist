using System.Web.Mvc;
using System.Configuration;


namespace CW.Soloist.WebApplication.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FileServer()
        {
            string fileServerAppDataUrl = ConfigurationManager.AppSettings["FileServerAppDataUrl"];
            return Redirect(fileServerAppDataUrl);
        }
    }
}