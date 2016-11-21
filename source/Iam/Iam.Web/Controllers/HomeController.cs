#region

using System.Web;
using System.Web.Mvc;
using Iam.Web.Plumbing;

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

        [AllowAnonymous]
        public ActionResult Impersonate()
        {
            Request.GetOwinContext().Set("Impersonate", "6d3000e7999240d9a0b8190fb73f65c9");
            return new ChallengeResult("OpenIdConnect", "https://nebula.iam.dev:44300/");
        }
    }
}