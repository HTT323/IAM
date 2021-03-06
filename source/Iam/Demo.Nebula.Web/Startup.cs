﻿#region

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using Demo.Nebula.Web;
using IdentityModel.Client;
using IdentityServer3.Core;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

#endregion

[assembly: OwinStartup(typeof(Startup))]

namespace Demo.Nebula.Web
{
    public class Startup
    {
        [UsedImplicitly]
        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    Authority = "https://auth.iam.dev:44300",
                    ClientId = "nebula-portal",
                    ResponseType = "id_token token",
                    SignInAsAuthenticationType = "Cookies",
                    Scope = "openid profile email role nebula-api-scope",
                    RedirectUri = "https://www.nebula-portal.dev:44320/",
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        SecurityTokenValidated = async n =>
                        {
                            var nid =
                                new ClaimsIdentity(
                                    n.AuthenticationTicket.Identity.AuthenticationType,
                                    Constants.ClaimTypes.Email,
                                    Constants.ClaimTypes.Role);

                            var sub = n.AuthenticationTicket.Identity.Claims
                                .First(f => f.Type == "sub").Value;

                            nid.AddClaim(new Claim("sub", sub));
                            nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                            nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                            var uic = new UserInfoClient(new Uri("https://auth.iam.dev:44300/connect/userinfo"), n.ProtocolMessage.AccessToken);
                            var ui = await uic.GetAsync();

                            foreach (var claim in ui.Claims)
                            {
                                if (nid.Claims.FirstOrDefault(f => f.Type == claim.Item1) == null)
                                    nid.AddClaim(new Claim(claim.Item1, claim.Item2));
                            }

                            n.AuthenticationTicket =
                                new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);
                        }
                    }
                });
        }
    }
}