
namespace JOERP.WebSite.Controllers
{
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Sesion");
        }
    }
}
