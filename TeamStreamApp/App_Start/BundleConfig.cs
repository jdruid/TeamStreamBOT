using System.Web;
using System.Web.Optimization;

namespace TeamStreamApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap/bootstrap.js"));

            bundles.Add(new StyleBundle("~/bundles/bootstrap-css")
                .IncludeDirectory("~/Styles/bootstrap/", "*.css"));
        }
    }
}
