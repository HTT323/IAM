#region

using System.Web.Mvc;

#endregion

namespace Iam.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}