﻿#region

using System.Web.Mvc;

#endregion

namespace Iam.Orion.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }
    }
}