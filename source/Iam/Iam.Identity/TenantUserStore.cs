#region

using JetBrains.Annotations;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class TenantUserStore : UserStore<IamUser>
    {
        internal TenantContext TenantContext { get; }

        // ReSharper disable once SuggestBaseTypeForParameter
        // The parameter must be of type TenantContext!
        public TenantUserStore(TenantContext context) : base(context)
        {
            TenantContext = context;
        }
    }
}