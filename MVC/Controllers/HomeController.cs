using System.Web.Mvc;

namespace PuntoDeVenta.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
