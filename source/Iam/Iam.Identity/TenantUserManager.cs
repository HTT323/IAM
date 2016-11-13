#region

using JetBrains.Annotations;
using Microsoft.AspNet.Identity;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class TenantUserManager : UserManager<IamUser>
    {
        internal TenantUserStore TenantUserStore { get; }
        
        public TenantUserManager(TenantUserStore store)
            : base(store)
        {
            TenantUserStore = store;
        }
    }
}