#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
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
            var clients = new List<Client>();

            clients.AddRange(DataFileLoader.Load<Client>("clients.json"));

            AddTestApiClient(clients);

            context.Clients.AddOrUpdate(c => c.ClientId,
                clients.Select(m => m.ToEntity()).ToArray());
        }

        [Conditional("DEBUG")]
        private void AddTestApiClient(List<Client> clients)
        {
            clients.Add(new Client
            {
                ClientId = "nebula-api-client",
                ClientName = "Nebula API Client (Service Communication)",
                Flow = Flows.ClientCredentials,
                ClientSecrets = new List<Secret>
                {
                    new Secret("nebula-client-secret".Sha256())
                },
                AllowedScopes = new List<string>
                {
                    "nebula-api-scope"
                }
            });
        }
    }
}