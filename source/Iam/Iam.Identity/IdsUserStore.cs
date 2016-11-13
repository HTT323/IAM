#region

using JetBrains.Annotations;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IdsUserStore : UserStore<IamUser>
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // The parameter must be of type IamContext!
        public IdsUserStore(IdsContext context) : base(context)
        {
        }
    }
}