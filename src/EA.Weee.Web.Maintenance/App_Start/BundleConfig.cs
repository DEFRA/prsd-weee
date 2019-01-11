namespace EA.Weee.Web.Maintenance
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery-3").Include(
                        "~/Scripts/jquery-3.3.1.js",
                        "~/Scripts/jquery-migrate-3.0.0.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.select-to-autocomplete.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/custom-validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_frontend").Include(
                        "~/Scripts/govuk_frontend/govuk-frontend-2.3.0.min.js",
                        "~/Scripts/govuk_frontend/html5shiv.js",
                        "~/Scripts/setup-govuk-frontend.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_weee").Include(
                        "~/Scripts/vendor/modernizr.custom.77028.js",
                        "~/Scripts/weee-application.js"));

           bundles.Add(new StyleBundle("~/Content/weee-page-ie6").Include(
                        "~/Content/weee-page-ie6.css"));

            bundles.Add(new StyleBundle("~/Content/weee-page-ie7").Include(
                        "~/Content/weee-page-ie7.css"));

            bundles.Add(new StyleBundle("~/Content/weee-page-ie8").Include(
                        "~/Content/weee-page-ie8.css"));

            bundles.Add(new StyleBundle("~/Content/weee-page").Include(
                        "~/Content/weee-page.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
