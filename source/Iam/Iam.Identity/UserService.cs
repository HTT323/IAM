#region

using System.Threading.Tasks;
using IdentityServer3.AspNetIdentity;
using IdentityServer3.Core.Models;
using JetBrains.Annotations;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class UserService : AspNetIdentityUserService<IamUser, string>
    {
        public UserService(IamUserManager userManager) : base(userManager)
        {
        }

        protected override Task<AuthenticateResult> PostAuthenticateLocalAsync(IamUser user, SignInMessage message)
        {
            return base.PostAuthenticateLocalAsync(user, message);
        }
    }
}