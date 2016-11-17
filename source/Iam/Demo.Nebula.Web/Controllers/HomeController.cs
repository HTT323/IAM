#region

using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityModel.Client;
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
        public async Task<ActionResult> Claims()
        {
            var user = User as ClaimsPrincipal;

            if (user == null)
                throw new InvalidOperationException();

            var accessToken = user.Claims.First(f => f.Type == "access_token").Value;
            var uic = new UserInfoClient(new Uri("https://auth.iam.dev:44300/connect/userinfo"), accessToken);
            var ui = await uic.GetAsync();

            ViewBag.Json = ui.JsonObject;

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

        [Authorize]
        public async Task<ActionResult> SecuredApiCcViewer()
        {
            var client = new TokenClient(
                "https://auth.iam.dev:44300/connect/token",
                "nebula-service",
                "nebula-api-access");

            var ccToken = await client.RequestClientCredentialsAsync("nebula-api-scope");

            var httpClient = new HttpClient();

            httpClient.SetBearerToken(ccToken.AccessToken);

            var json = await httpClient.GetStringAsync("https://localhost:44330/identity");

            ViewBag.Json = JArray.Parse(json).ToString();

            return View();
        }
    }
}