#region

using System;
using System.Threading.Tasks;
using IdentityServer3.WsFederation.Models;
using IdentityServer3.WsFederation.Services;
using JetBrains.Annotations;

#endregion

namespace Iam.Web.Services
{
    [UsedImplicitly]
    public class RelyingPartyService : IRelyingPartyService
    {
        public Task<RelyingParty> GetByRealmAsync(string realm)
        {
            throw new NotImplementedException();
        }
    }
}