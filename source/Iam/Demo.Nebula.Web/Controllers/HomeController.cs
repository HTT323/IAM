#region

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

#endregion

namespace Demo.Nebula.Web.Controllers
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

        [Authorize]
        public async Task<ActionResult> SecuredApiViewer()
        {
            var user = User as ClaimsPrincipal;

            if (user == null)
                throw new InvalidOperationException();

            var accessToken = user.Claims.First(f => f.Type == "access_token").Value;
            var httpClient = new HttpClient();

            httpClient.SetBearerToken(accessToken);

            var json = await httpClient.GetStringAsync("https://localhost:44330/identity");

            ViewBag.Json = JArray.Parse(json).ToString();

            return View();
        }
    }
}