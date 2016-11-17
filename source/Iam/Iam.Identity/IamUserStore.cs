#region

using JetBrains.Annotations;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IamUserStore : UserStore<IamUser>
    {
        internal IamContext IdsContext { get; }

        public IamUserStore(IamContext context) : base(context)
        {
            IdsContext = context;
        }
    }
}