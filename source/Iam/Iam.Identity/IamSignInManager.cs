#region

using JetBrains.Annotations;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IamSignInManager : SignInManager<IamUser, string>
    {
        public IamSignInManager(IamUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
    }
}