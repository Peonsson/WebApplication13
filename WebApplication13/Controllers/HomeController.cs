using System.Web.Mvc;

namespace WebApplication13.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Chat()
        {
            ViewBag.Title = "Chat Page";

            return View();
        }

        public ActionResult ChatBody()
        {
            ViewBag.Title = "ChatBody Page";

            return View();
        }
    }
}
