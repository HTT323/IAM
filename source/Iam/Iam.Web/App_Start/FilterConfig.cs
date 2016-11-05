#region

using System.Web.Mvc;
using JetBrains.Annotations;

#endregion

namespace Iam.Web
{
    [UsedImplicitly]
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}