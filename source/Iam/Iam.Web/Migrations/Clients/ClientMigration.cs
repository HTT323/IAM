#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

            context.Clients.AddOrUpdate(c => c.ClientId,
                clients.Select(m => m.ToEntity()).ToArray());
        }
    }
}