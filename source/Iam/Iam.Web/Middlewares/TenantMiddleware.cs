#region

using System;
using System.Threading.Tasks;
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
            var parts = context.Request.Host.Value.Split('.');

            if (parts.Length != 3)
                throw new InvalidOperationException();

            var sd = parts[0].ToLower();

            context.Set(TenantKey, sd);

            await Next.Invoke(context);
        }
    }
}