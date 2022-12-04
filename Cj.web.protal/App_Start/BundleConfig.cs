using System.Web;
using System.Web.Optimization;

namespace Cj.web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.js",
                         "~/Scripts/bootstrap.bundle.min.js",
                        "~/Scripts/jquery.easing.min.js",
                        "~/Scripts/sb-admin-2.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                  "~/Content/sb-admin-2.css",
                  "~/Content/font-awesome.css"
                ));
        }
    }
}