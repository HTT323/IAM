#region

using System;
using System.Web.Mvc;

#endregion

namespace Iam.Web.Controllers
{
    public class ManageController : AbstractController
    {
        public ActionResult Index(string tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant))
                throw new ArgumentNullException(nameof(tenant));

            return View();
        }
    }
}