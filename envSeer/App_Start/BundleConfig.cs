using System.Web;
using System.Web.Optimization;

namespace envSeer
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Content/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/Styles/bootstrap.css",
                      "~/Content/Styles/font-awesome.min.css",
                      "~/Content/Styles/site.css"));


            // creating bundles for custom css & js
            bundles.Add(new ScriptBundle("~/Custom/Scripts").Include(
                       "~/Content/Scripts/Custom Scripts/envSeerScripts.js"));

            bundles.Add(new StyleBundle("~/Custom/Styles").Include(
                        "~/Content/Styles/Custom Styles/envSeerStyles.css"));
        }
    }
}
