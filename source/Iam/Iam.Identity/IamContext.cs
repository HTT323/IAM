#region

using Iam.Common;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IamContext : IdentityDbContext<IamUser>
    {
        public IamContext() : base(AppSettings.IamConnectionString)
        {
        }

        public IamContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public static IamContext Create()
        {
            return new IamContext();
        }
    }
}