using System.Web;
using System.Web.Optimization;

namespace Overseer.WebApp
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
                        "~/Content/Styles/font-awesome.min.css",
                        "~/Content/Styles/site.css"));

            // bundle for custom js
            bundles.Add(new ScriptBundle("~/Custom/Scripts").Include(
                        "~/Content/Scripts/Custom Scripts/OverseerScripts.js",
                        "~/Content/Scripts/Custom Scripts/ToggleField.js"));

            // bundle for scripts used in profile pages
            bundles.Add(new ScriptBundle("~/UserProfile/Scripts").Include(
                        "~/Content/Scripts/jquery.validate.js",
                        "~/Content/Scripts/jquery.validate.unobtrusive.js",
                        "~/Content/Scripts/Custom Scripts/UserProfileAjax.js"
                ));

            // bundle for scripts used in environment pages
            bundles.Add(new ScriptBundle("~/Environment/Scripts").Include(
                        "~/Content/Scripts/jquery.validate.js",
                        "~/Content/Scripts/jquery.validate.unobtrusive.js",
                        "~/Content/Scripts/Custom Scripts/EnvironmentAjax.js"
                ));

            // bundle for 'Machineseer' page
            bundles.Add(new ScriptBundle("~/Machineseer/Scripts").Include(
                        "~/Content/Scripts/Custom Scripts/RefreshMonitoringData.js",
                        "~/Content/Scripts/Custom Scripts/ContentAccordion.js",
                        "~/Content/Scripts/Chart.js"
                ));

            // bundle for 'Environmentseer' page
            bundles.Add(new ScriptBundle("~/Environmentseer/Scripts").Include(
                        "~/Content/Scripts/Custom Scripts/ContentAccordion.js",
                        "~/Content/Scripts/Chart.js"
                ));

            // bundles for scripts used in specific pages
            bundles.Add(new ScriptBundle("~/Environment/MachineConfiguration/Scripts").Include(
                        "~/Content/Scripts/Custom Scripts/EditableListBox.js",
                        "~/Content/Scripts/Custom Scripts/DynamicAlertsConfig.js"
                ));

            // bundle for custom site styles
            bundles.Add(new StyleBundle("~/Custom/Styles").Include(
                        "~/Content/Styles/Custom Styles/OverseerStyles.css"));
        }
    }
}
