#region

using System.Web.Mvc;

#endregion

namespace Iam.Web.Controllers
{
    public class HomeController : AbstractController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}