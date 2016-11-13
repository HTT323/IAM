#region

using JetBrains.Annotations;
using Microsoft.AspNet.Identity;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IdsUserManager : UserManager<IamUser>
    {
        public IdsUserManager(IdsUserStore store)
            : base(store)
        {
        }
    }
}