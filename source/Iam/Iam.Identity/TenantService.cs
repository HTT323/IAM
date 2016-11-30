#region

using System.Collections.Generic;
using System.Linq;
using Iam.Common;
using Iam.Common.Contracts;
using Iam.Identity.Tenant;
using JetBrains.Annotations;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class TenantService
    {
        private const string GetAllCacheKey = "TenantService.GetAll";
        private const string GetAllWsFedCacheKey = "TenantService.GetAllWsFed";
        private readonly ICache _cache;
        private readonly IAdminContext _context;

        public TenantService(IAdminContext context, ICache cache)
        {
            _context = context;
            _cache = cache;
        }

        public TenantMapping GetIamMapping(string tenantId, string clientId)
        {
            var list = GetAll();
            var mapping = list.FirstOrDefault(f => f.TenantId == tenantId && f.ClientId == clientId);

            Ensure.NotNull(mapping);

            return mapping;
        }

        public TenantMapping GetClientMapping(string clientId)
        {
            var list = GetAll();
            var mapping = list.FirstOrDefault(f => f.ClientId == clientId);

            Ensure.NotNull(mapping);

            return mapping;
        }

        public IEnumerable<WsFedMapping> GetAllWsFed()
        {
            var data = _cache.Get(GetAllWsFedCacheKey) as IEnumerable<WsFedMapping>;

            if (data != null)
                return data;

            var list = _context.Repository<WsFedMapping>().ToList();

            _cache.Put(GetAllWsFedCacheKey, list);

            return list;
        }

        public WsFedProtocolMapping GetWsFedProtocolMappingByRealm(string realm)
        {
            var mapping = 
                _context.Repository<WsFedProtocolMapping>().FirstOrDefault(f => f.Realm == realm);

            Ensure.NotNull(mapping);

            return mapping;
        }

        private IEnumerable<TenantMapping> GetAll()
        {
            var data = _cache.Get(GetAllCacheKey) as IEnumerable<TenantMapping>;

            if (data != null)
                return data;

            var list = _context.Repository<TenantMapping>().ToList();

            _cache.Put(GetAllCacheKey, list);

            return list;
        }
    }
}