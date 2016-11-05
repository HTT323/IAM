#region

using System;
using Iam.Common;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class IamUserManager : UserManager<IamUser>
    {
        private const string DataProtectionPurpose = "Identity and Access Management";

        public IamUserManager(IamUserStore store)
            : base(store)
        {
        }

        public static IamUserManager Create(IdentityFactoryOptions<IamUserManager> options,
            IOwinContext context)
        {
            var manager = new IamUserManager(new IamUserStore(context.Get<IamContext>()));

            manager.UserValidator = new UserValidator<IamUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = AppSettings.RequiredLength,
                RequireNonLetterOrDigit = AppSettings.RequireNonLetterOrDigit,
                RequireDigit = AppSettings.RequireDigit,
                RequireLowercase = AppSettings.RequireLowercase,
                RequireUppercase = AppSettings.RequireUppercase
            };

            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            var dataProtectionProvider = options.DataProtectionProvider;

            if (dataProtectionProvider != null)
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<IamUser>(
                        dataProtectionProvider.Create(DataProtectionPurpose));

            return manager;
        }
    }
}