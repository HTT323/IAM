#region

using System.Web;
using System.Web.Routing;

#endregion

namespace Iam.Nebula.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}