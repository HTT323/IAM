#region

using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    public class IamRoleStore : RoleStore<IdentityRole>
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        // The parameter must be of type IamContext!
        public IamRoleStore(IamContext context) : base(context)
        {
        }
    }
}