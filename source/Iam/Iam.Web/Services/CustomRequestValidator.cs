#region

using System;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;
using JetBrains.Annotations;

#endregion

namespace Iam.Web.Services
{
    [UsedImplicitly]
    public class CustomRequestValidator : ICustomRequestValidator
    {
        public Task<AuthorizeRequestValidationResult> ValidateAuthorizeRequestAsync(ValidatedAuthorizeRequest request)
        {
            if (!request.Subject.Identity.IsAuthenticated)
                return Task.FromResult(new AuthorizeRequestValidationResult { IsError = false });



            return Task.FromResult(new AuthorizeRequestValidationResult {IsError = false});
        }

        public Task<TokenRequestValidationResult> ValidateTokenRequestAsync(ValidatedTokenRequest request)
        {
            throw new NotImplementedException();
        }
    }
}