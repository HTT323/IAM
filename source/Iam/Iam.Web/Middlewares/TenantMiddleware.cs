#region

using System.Threading.Tasks;
using Iam.Common;
using JetBrains.Annotations;
using Microsoft.Owin;

#endregion

namespace Iam.Web.Middlewares
{
    [UsedImplicitly]
    public class TenantMiddleware : OwinMiddleware
    {
        internal const string TenantKey = "Tenant";

        public TenantMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            SetTenant(context);

            await Next.Invoke(context);
        }

        private static void SetTenant(IOwinContext context)
        {
            var parts = context.Request.Host.Value.Split('.');

            Ensure.Equal(parts.Length, 3);

            var sd = parts[0].ToLower();

            context.Set(TenantKey, sd);
        }
    }
}