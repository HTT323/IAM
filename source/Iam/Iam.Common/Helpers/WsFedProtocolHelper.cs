#region

using System;
using System.Diagnostics;
using System.Linq;
using System.Web;

#endregion

namespace Iam.Common.Helpers
{
    public static class WsFedProtocolHelper
    {
        public static bool IsWsFedProtocolRequest(string returnUrl)
        {
            var uri = new Uri(returnUrl);

            Trace.TraceInformation("Check WsFed request URI absolute path: {0}", uri.AbsolutePath);

            return uri.AbsolutePath == $"{AppSettings.IdsAppPath}/wsfed";
        }

        public static string GetRealm(string returnUrl)
        {
            var uri = new Uri(returnUrl);
            var qs = HttpUtility.ParseQueryString(uri.Query);
            var key = qs.AllKeys.FirstOrDefault(f => f == "wtrealm");

            Ensure.NotNull(key);

            var realm = qs[key];

            Ensure.NotNullOrEmpty(realm);

            return realm;
        }
    }
}