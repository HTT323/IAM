#region

using Iam.Common;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IdsContext : IdentityDbContext<IamUser>
    {
        public IdsContext() : base(AppSettings.IdsConnectionString)
        {
        }
    }
}