#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Iam.Common;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;

#endregion

namespace Iam.Web.Migrations.Clients
{
    public class ClientMigration : DbMigrationsConfiguration<ClientConfigurationDbContext>
    {
        public ClientMigration()
        {
            MigrationsDirectory = @"Migrations\Clients";
        }

        protected override void Seed(ClientConfigurationDbContext context)
        {
            var clients = new List<Client>
            {
                new Client
                {
                    ClientId = AppSettings.IamClientId,
                    ClientName = AppSettings.IamClientName,
                    Flow = Flows.Implicit,
                    RequireConsent = false,
                    RedirectUris = new List<string>
                    {
                        "https://orion.iam.dev:44300/",
                        "https://nebula.iam.dev:44300/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://orion.iam.dev:44300/",
                        "https://nebula.iam.dev:44300/"
                    },
                    IdentityProviderRestrictions = new List<string> {Constants.PrimaryAuthenticationType},
                    AllowedScopes =
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email
                    }
                }
            };

            context.Clients.AddOrUpdate(c => c.ClientId,
                clients.Select(m => m.ToEntity()).ToArray());
        }
    }
}