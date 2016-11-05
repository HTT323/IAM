#region

using System.Web.Optimization;
using JetBrains.Annotations;

#endregion

namespace Iam.Web
{
    [UsedImplicitly]
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/scripts/jquery-{version}.js", 
                "~/scripts/bootstrap.js", 
                "~/scripts/angular.js", 
                "~/scripts/underscore.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/content/bootstrap.css",
                "~/content/site.css"));
        }
    }
}