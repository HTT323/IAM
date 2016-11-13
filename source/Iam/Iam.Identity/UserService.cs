#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Iam.Common;
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
        private readonly TenantUserManager _tenantUserManager;
        private readonly TenantService _tenantService;

        public UserService(IdsUserManager userManager, TenantUserManager tenantUserManager, TenantService tenantService)
            : base(userManager)
        {
            _tenantUserManager = tenantUserManager;
            _tenantService = tenantService;
        }

        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext ctx)
        {
            var clientId = ctx.SignInMessage.ClientId;
            var tenant = ctx.SignInMessage.Tenant;

            if (clientId == AppSettings.IamClientId && tenant == AppSettings.AdminDomain)
            {
                await base.AuthenticateLocalAsync(ctx);
                return;
            }

            if (clientId == AppSettings.IamClientId && tenant != AppSettings.AdminDomain)
            {
                _tenantUserManager.TenantUserStore.TenantContext.CacheKey = tenant;
            }
            else
            {
                var mapping = _tenantService.GetClientMapping(clientId);

                if (mapping == null)
                    throw new InvalidOperationException("Invalid tenant mapping");

                _tenantUserManager.TenantUserStore.TenantContext.CacheKey = mapping.TenantId;
            }

            var username = ctx.UserName;
            var password = ctx.Password;
            var message = ctx.SignInMessage;

            ctx.AuthenticateResult = null;

            if (_tenantUserManager.SupportsUserPassword)
            {
                var user = await FindUserAsync(_tenantUserManager, username);

                if (user != null)
                {
                    if (_tenantUserManager.SupportsUserLockout &&
                        await _tenantUserManager.IsLockedOutAsync(user.Id))
                    {
                        return;
                    }

                    if (await _tenantUserManager.CheckPasswordAsync(user, password))
                    {
                        if (_tenantUserManager.SupportsUserLockout)
                        {
                            await _tenantUserManager.ResetAccessFailedCountAsync(user.Id);
                        }

                        var result = await PostAuthenticateLocalAsync(user, message);

                        if (result == null)
                        {
                            var claims = await GetClaimsForAuthenticateResult(_tenantUserManager, user);

                            result =
                                new AuthenticateResult(
                                    user.Id,
                                    await GetDisplayNameForAccountAsync(_tenantUserManager, user.Id), claims);
                        }

                        ctx.AuthenticateResult = result;
                    }
                    else if (_tenantUserManager.SupportsUserLockout)
                    {
                        await _tenantUserManager.AccessFailedAsync(user.Id);
                    }
                }
            }
        }

        private async Task<string> GetDisplayNameForAccountAsync(
            TenantUserManager tenantUserManager,
            string userId)
        {
            var user = await tenantUserManager.FindByIdAsync(userId);
            var claims = await GetClaimsFromAccount(tenantUserManager, user);

            Claim nameClaim = null;

            var claimsList = claims as IList<Claim> ?? claims.ToList();

            if (DisplayNameClaimType != null)
            {
                nameClaim = claimsList.FirstOrDefault(x => x.Type == DisplayNameClaimType);
            }

            if (nameClaim == null) nameClaim = claimsList.FirstOrDefault(x => x.Type == Constants.ClaimTypes.Name);
            if (nameClaim == null) nameClaim = claimsList.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            return nameClaim != null ? nameClaim.Value : user.UserName;
        }

        private static async Task<IEnumerable<Claim>> GetClaimsFromAccount(
            TenantUserManager tenantUserManager,
            IamUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(Constants.ClaimTypes.Subject, user.Id),
                new Claim(Constants.ClaimTypes.PreferredUserName, user.UserName)
            };

            if (tenantUserManager.SupportsUserEmail)
            {
                var email = await tenantUserManager.GetEmailAsync(user.Id);

                if (!string.IsNullOrWhiteSpace(email))
                {
                    claims.Add(new Claim(Constants.ClaimTypes.Email, email));

                    var verified = await tenantUserManager.IsEmailConfirmedAsync(user.Id);

                    claims.Add(new Claim(Constants.ClaimTypes.EmailVerified, verified ? "true" : "false"));
                }
            }

            if (tenantUserManager.SupportsUserPhoneNumber)
            {
                var phone = await tenantUserManager.GetPhoneNumberAsync(user.Id);

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    claims.Add(new Claim(Constants.ClaimTypes.PhoneNumber, phone));

                    var verified = await tenantUserManager.IsPhoneNumberConfirmedAsync(user.Id);

                    claims.Add(new Claim(Constants.ClaimTypes.PhoneNumberVerified, verified ? "true" : "false"));
                }
            }

            if (tenantUserManager.SupportsUserClaim)
            {
                claims.AddRange(await tenantUserManager.GetClaimsAsync(user.Id));
            }

            if (!tenantUserManager.SupportsUserRole) return claims;

            var roleClaims =
                from role in await tenantUserManager.GetRolesAsync(user.Id)
                select new Claim(Constants.ClaimTypes.Role, role);

            claims.AddRange(roleClaims);

            return claims;
        }

        private async Task<IEnumerable<Claim>> GetClaimsForAuthenticateResult(
            TenantUserManager tenantUserManager,
            IamUser user)
        {
            var claims = new List<Claim>();

            if (!EnableSecurityStamp || !tenantUserManager.SupportsUserSecurityStamp) return claims;

            var stamp = await tenantUserManager.GetSecurityStampAsync(user.Id);

            if (!string.IsNullOrWhiteSpace(stamp))
            {
                claims.Add(new Claim("security_stamp", stamp));
            }

            return claims;
        }

        private static async Task<IamUser> FindUserAsync(
            TenantUserManager tenantUserManager,
            string username)
        {
            return await tenantUserManager.FindByNameAsync(username);
        }
    }
}