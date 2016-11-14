#region

using System.Web;
using System.Web.Routing;
using JetBrains.Annotations;

#endregion

namespace Demo.Orion.Web
{
    [UsedImplicitly]
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {   
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}