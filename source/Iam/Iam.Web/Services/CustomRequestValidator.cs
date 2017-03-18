#region

using System;
using System.Linq;
using System.Threading.Tasks;
using Iam.Common;
using Iam.Identity;
using Iam.Identity.Tenant;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;
using JetBrains.Annotations;

#endregion

namespace Iam.Web.Services
{
    [UsedImplicitly]
    public class CustomRequestValidator : ICustomRequestValidator
    {
        private readonly TenantService _tenantService;

        public CustomRequestValidator(TenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public Task<AuthorizeRequestValidationResult> ValidateAuthorizeRequestAsync(ValidatedAuthorizeRequest request)
        {
            if (!request.Subject.Identity.IsAuthenticated)
                return Task.FromResult(new AuthorizeRequestValidationResult {IsError = false});

            var tenantId = request.Subject.Claims.First(f => f.Type == "tenant_mapping").Value;

            TenantMapping mapping;

            // Extract the tenant info from acr if the client is IAM, get it from claims otherwise.
            if (request.ClientId == AppSettings.IamClientId)
            {
                var iamTenantId =
                    request.AuthenticationContextReferenceClasses.First(f => f.StartsWith("tenant:"))
                        .Replace("tenant:", string.Empty);

                mapping = _tenantService.GetIamMapping(iamTenantId, request.ClientId);
            }
            else
            {
                mapping = _tenantService.GetClientMapping(request.ClientId);
            }

            if (tenantId == mapping.TenantId)
                return Task.FromResult(new AuthorizeRequestValidationResult {IsError = false});

            request.PromptMode = "login";

            var result = new AuthorizeRequestValidationResult {IsError = false, ValidatedRequest = request};

            return Task.FromResult(result);
        }

        public Task<TokenRequestValidationResult> ValidateTokenRequestAsync(ValidatedTokenRequest request)
        {
            return Task.FromResult(new TokenRequestValidationResult {IsError = false});
        }
    }
}