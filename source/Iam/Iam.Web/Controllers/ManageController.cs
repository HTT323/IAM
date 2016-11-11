#region

using System.Web.Mvc;

#endregion

namespace Iam.Web.Controllers
{
    public class ManageController : AbstractController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}