#region

using JetBrains.Annotations;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IamRoleStore : RoleStore<IdentityRole>
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // The parameter must be of type IamContext!
        public IamRoleStore(IamContext context) : base(context)
        {
        }
    }
}