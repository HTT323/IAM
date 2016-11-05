#region

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    public class IamRoleManager : RoleManager<IdentityRole>
    {
        public IamRoleManager(IamRoleStore store) : base(store)
        {
        }
    }
}