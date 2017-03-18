#region

using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityModel.Constants;
using Thinktecture.IdentityModel.WSTrust;

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
            var name = User.Identity.Name;

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

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            
            var json = await httpClient.GetStringAsync("https://localhost:44330/identity");

            ViewBag.Json = JArray.Parse(json).ToString();

            return View();
        }

        public ActionResult GetCookie()
        {
            var httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                CookieContainer = new CookieContainer(),
                Credentials = new NetworkCredential("Administrator", "Abcde12345#", "gatherforms")
            };

            using (var client = new HttpClient(httpClientHandler))
            {
                client.BaseAddress = new Uri("https://sp2013.gatherforms.org");
                
                var response = client.PostAsync("/", null).Result;
                var cookies = response.Headers.First(f => f.Key == "Set-Cookie");
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }

        [Authorize]
        public ActionResult SpTrust()
        {
            var cp = User as ClaimsPrincipal;

            if (cp == null)
                throw new InvalidOperationException();

            var email = cp.Claims.First(f => f.Type == "email").Value;
            var accessToken = cp.Claims.First(f => f.Type == "access_token").Value;
            
            const string realm = "urn:nebula:8af89396db32459c8cf2a819f1142c36";
            const string ep = "https://localhost:44303/issue/wstrust/mixed/username";

            var factory =
                new WSTrustChannelFactory(
                    new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                    new EndpointAddress(ep))
                {
                    TrustVersion = TrustVersion.WSTrust13
                };

            if (factory.Credentials == null)
                throw new InvalidOperationException();

            factory.Credentials.UserName.UserName = email;
            factory.Credentials.UserName.Password = accessToken;

            var channel = factory.CreateChannel();

            var securityToken = new RequestSecurityToken
            {
                TokenType = TokenTypes.OasisWssSaml11TokenProfile11,
                AppliesTo = new EndpointReference(realm),
                RequestType = RequestTypes.Issue,
                KeyType = KeyTypes.Bearer
            };

            var message =
                channel.Issue(Message.CreateMessage(MessageVersion.Default, WSTrust13Constants.Actions.Issue,
                    new WSTrustRequestBodyWriter(securityToken, new WSTrust13RequestSerializer(),
                        new WSTrustSerializationContext())));

            var reader = message.GetReaderAtBodyContents();
            var token = reader.ReadOuterXml();

            const string assert = @"<trust:RequestSecurityTokenResponse xmlns:trust=""http://docs.oasis-open.org/ws-sx/ws-trust/200512"">";
            const string assertStart = "<trust:RequestSecurityTokenResponse>";
            const string assertEnd = "</trust:RequestSecurityTokenResponse>";

            var startAssertIndex = token.IndexOf(assertStart, StringComparison.InvariantCulture);
            token = token.Substring(startAssertIndex);

            var endAssertIndex = token.IndexOf(assertEnd, StringComparison.InvariantCulture) + assertEnd.Length;
            token = token.Substring(0, endAssertIndex);
            token = token.Replace(assertStart, assert);

            var cookies = GetAuthenticationCookie(token);

            return Content(token, "text/xml");
        }

        private static IEnumerable<string> GetAuthenticationCookie(string token)
        {
            const string wa = "wsignin1.0";
            const string wctx = "/_layouts/Authenticate.aspx?Source=%2F";

            var httpClientHandler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                CookieContainer = new CookieContainer()
            };

            using (var client = new HttpClient(httpClientHandler))
            {
                client.BaseAddress = new Uri("https://sp2013.gatherforms.org");

                var data =
                    new FormUrlEncodedContent(
                        new[]
                        {
                            new KeyValuePair<string, string>("wa", wa),
                            new KeyValuePair<string, string>("wresult", token),
                            new KeyValuePair<string, string>("wctx", wctx)
                        });

                var response = client.PostAsync("/_trust/", data).Result;
                var cookies = response.Headers.First(f => f.Key == "Set-Cookie");

                return cookies.Value;
            }
        }

        [Authorize]
        public async Task<ActionResult> SecuredApiCcViewer()
        {
            var client =
                new TokenClient(
                    "https://auth.iam.dev:44300/connect/token",
                    "nebula-service",
                    "nebula-api-access");

            var ccToken = await client.RequestClientCredentialsAsync("nebula-api-scope");

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {ccToken.AccessToken}");

            var json = await httpClient.GetStringAsync("https://localhost:44330/identity");

            ViewBag.Json = JArray.Parse(json).ToString();

            return View();
        }
    }
}