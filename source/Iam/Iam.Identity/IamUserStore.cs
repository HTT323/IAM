#region

using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    public class IamUserStore : UserStore<IamUser>
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // The parameter must be of type IamContext!
        public IamUserStore(IamContext context) : base(context)
        {
        }
    }
}