#region

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Web.Http;
using Demo.Nebula.Api;
using IdentityServer3.AccessTokenValidation;
using JetBrains.Annotations;
using Microsoft.Owin;
using Owin;

#endregion

[assembly: OwinStartup(typeof(Startup))]

namespace Demo.Nebula.Api
{
    public class Startup
    {
        [UsedImplicitly]
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseIdentityServerBearerTokenAuthentication(
                new IdentityServerBearerTokenAuthenticationOptions
                {
                    Authority = "https://auth.iam.dev:44300",
                    RequiredScopes = new[] {"nebula-api-scope"}
                });

            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Nebula API home with one secured endpoint -> /identity");
            });
        }
    }
}