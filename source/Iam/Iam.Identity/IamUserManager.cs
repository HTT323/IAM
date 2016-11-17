#region

using JetBrains.Annotations;
using Microsoft.AspNet.Identity;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IamUserManager : UserManager<IamUser>
    {
        internal IamUserStore IdsUserStore { get; }

        public IamUserManager(IamUserStore store)
            : base(store)
        {
            IdsUserStore = store;
        }
    }
}