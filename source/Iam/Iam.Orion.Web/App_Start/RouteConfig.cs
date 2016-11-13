#region

using System.Web.Mvc;
using System.Web.Routing;
using JetBrains.Annotations;

#endregion

namespace Iam.Orion.Web
{
    [UsedImplicitly]
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}