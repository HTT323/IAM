﻿#region

using System.Collections.Generic;
using System.Linq;
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

            return list.FirstOrDefault(f => f.TenantId == tenantId && f.ClientId == clientId);
        }

        public TenantMapping GetClientMapping(string clientId)
        {
            var list = GetAll();

            return list.FirstOrDefault(f => f.ClientId == clientId);
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