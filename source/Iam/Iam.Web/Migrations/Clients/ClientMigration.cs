#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
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
        private bool _debug;

        public ClientMigration()
        {
            MigrationsDirectory = @"Migrations\Clients";
            SetDebugFlag();
        }

        protected override void Seed(ClientConfigurationDbContext context)
        {
            var clients = new List<Client>();

            if (_debug)
            {
                clients.AddRange(DataFileLoader.Load<Client>("clients.json"));
            }
            else
            {
                clients.Add(new Client
                {
                    ClientId = AppSettings.IamClientId,
                    ClientName = AppSettings.IamClientName,
                    Flow = Flows.Implicit,
                    RequireConsent = false,
                    RedirectUris = new List<string>
                    {
                        AppSettings.IamAdminFullUrl
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        AppSettings.IamAdminFullUrl
                    },
                    IdentityProviderRestrictions = new List<string>
                    {
                        Constants.PrimaryAuthenticationType
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "email",
                        "role"
                    }
                });
            }

            context.Clients.AddOrUpdate(c => c.ClientId,
                clients.Select(m => m.ToEntity()).ToArray());
        }

        [Conditional("DEBUG")]
        private void SetDebugFlag()
        {
            _debug = true;
        }
    }
}