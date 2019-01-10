namespace EA.Weee.Web
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // JQuery 1.x.x is used for IE 8 and earlier.
            bundles.Add(new ScriptBundle("~/bundles/jquery-1").Include(
                "~/Scripts/jquery-1*",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery.select-to-autocomplete.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/custom-validation.js"));

            // JQuery 3.x.x is used for allother browsers.
            bundles.Add(new ScriptBundle("~/bundles/jquery-3").Include(
                "~/Scripts/jquery-3.3.1.js",
                "~/Scripts/jquery-migrate-3.0.0.js",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery.select-to-autocomplete.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/custom-validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_frontend").Include(
                      "~/Scripts/govuk_frontend/govuk-frontend-2.3.0.min.js",
                      "~/Scripts/govuk_frontend/html5shiv.js",
                        "~/Scripts/setup-govuk-frontend.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_weee").Include(
                      "~/Scripts/vendor/modernizr.custom.77028.js",
                      "~/Scripts/vendor/details.polyfill.js",
                      "~/Scripts/weee-application.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_frontend_script").Include(
                "~/Scripts/govuk_frontend/govuk-frontend-2.3.0.min.js"));

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