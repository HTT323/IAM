﻿#region

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Iam.Common;
using Iam.Common.Helpers;
using IdentityModel;
using IdentityServer3.AspNetIdentity;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using JetBrains.Annotations;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class UserService : AspNetIdentityUserService<IamUser, string>
    {
        private readonly TenantService _tenantService;
        private string _schema;

        public UserService(
            IamUserManager userManager,
            TenantService tenantService)
            : base(userManager)
        {
            _tenantService = tenantService;
        }

        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext ctx)
        {
            var clientId = ctx.SignInMessage.ClientId;
            
            if (clientId == null)
            {
                var isWsfpRequest = WsFedProtocolHelper.IsWsFedProtocolRequest(ctx.SignInMessage.ReturnUrl);

                Ensure.Equal(isWsfpRequest, true);

                var realm = WsFedProtocolHelper.GetRealm(ctx.SignInMessage.ReturnUrl);
                var wsfp = _tenantService.GetWsFedProtocolMappingByRealm(realm);
                var mapping = _tenantService.GetClientMapping(wsfp.Realm);

                _schema = mapping.TenantId;
            }
            else if (clientId == AppSettings.IamClientId)
            {
                _schema = ctx.SignInMessage.Tenant;
            }
            else
            {
                var mapping = _tenantService.GetClientMapping(clientId);
                
                _schema = mapping.TenantId;
            }

            ((IamUserManager) userManager).IdsUserStore.IdsContext.CacheKey = _schema;

            await base.AuthenticateLocalAsync(ctx);
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext ctx)
        {
            string clientId = null;

            if (ctx.Client != null)
                clientId = ctx.Client.ClientId;

            ((IamUserManager) userManager).IdsUserStore.IdsContext.CacheKey =
                GetTenant(ctx.Subject, clientId);

            await base.GetProfileDataAsync(ctx);
        }

        public override async Task IsActiveAsync(IsActiveContext ctx)
        {
            string clientId = null;

            if (ctx.Client != null)
                clientId = ctx.Client.ClientId;

            ((IamUserManager) userManager).IdsUserStore.IdsContext.CacheKey =
                GetTenant(ctx.Subject, clientId);

            await base.IsActiveAsync(ctx);
        }

        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext ctx)
        {
            string schema;

            if (ctx.SignInMessage.ClientId == null)
            {
                var isWsfpRequest = WsFedProtocolHelper.IsWsFedProtocolRequest(ctx.SignInMessage.ReturnUrl);

                Ensure.Equal(isWsfpRequest, true);

                var realm = WsFedProtocolHelper.GetRealm(ctx.SignInMessage.ReturnUrl);
                var wsfp = _tenantService.GetWsFedProtocolMappingByRealm(realm);
                var mapping = _tenantService.GetClientMapping(wsfp.Realm);

                schema = mapping.TenantId;
            }
            else
            {
                schema = ctx.SignInMessage.ClientId == AppSettings.IamClientId
                    ? GetTenant(ctx.SignInMessage.Tenant, ctx.SignInMessage.ClientId)
                    : GetTenant(ctx.SignInMessage.ClientId);
            }

            ((IamUserManager) userManager).IdsUserStore.IdsContext.CacheKey = schema;

            _schema = schema;

            return base.AuthenticateExternalAsync(ctx);
        }

        protected override async Task<AuthenticateResult> UpdateAccountFromExternalClaimsAsync(
            string userId,
            string provider,
            string providerId,
            IEnumerable<Claim> claims)
        {
            var list = claims.ToList();
            var existingClaims = await userManager.GetClaimsAsync(userId);
            var intersection = existingClaims.Intersect(list, new ClaimComparer());
            var newClaims = list.Except(intersection, new ClaimComparer());

            foreach (var claim in newClaims)
            {
                var result = await userManager.AddClaimAsync(userId, claim);

                if (!result.Succeeded)
                {
                    return new AuthenticateResult(result.Errors.First());
                }
            }

            return null;
        }

        protected override async Task<IEnumerable<Claim>> GetClaimsForAuthenticateResult(IamUser user)
        {
            Ensure.Argument.NotNull(user, nameof(user));

            var claims = new List<Claim>();

            if (!EnableSecurityStamp || !userManager.SupportsUserSecurityStamp) return claims;

            var stamp = await userManager.GetSecurityStampAsync(user.Id);

            if (string.IsNullOrWhiteSpace(stamp)) return claims;

            Ensure.NotNull(_schema);

            claims.Add(new Claim("security_stamp", stamp));
            claims.Add(new Claim("tenant_mapping", _schema));

            return claims;
        }

        #region Tenants

        private string GetTenant(string clientId)
        {
            var tenantKey = _tenantService.GetClientMapping(clientId);

            Ensure.NotNull(tenantKey);

            return tenantKey.TenantId;
        }

        private string GetTenant(string tenantId, string clientId)
        {
            var tenantKey = _tenantService.GetIamMapping(tenantId, clientId);

            Ensure.NotNull(tenantKey);

            return tenantKey.TenantId;
        }

        private string GetTenant(ClaimsPrincipal subject, string clientId)
        {
            Ensure.Argument.NotNull(subject, nameof(subject));
            
            var tenant = subject.Claims.FirstOrDefault(f => f.Type == "tenant_mapping");
            var tenantKey = tenant?.Value;

            if (subject.Identity.AuthenticationType != Constants.PrimaryAuthenticationType)
            {
                tenantKey = _tenantService.GetClientMapping(clientId).TenantId;
            }

            Ensure.NotNullOrEmpty(tenantKey);

            return tenantKey;
        }

        #endregion
    }
}