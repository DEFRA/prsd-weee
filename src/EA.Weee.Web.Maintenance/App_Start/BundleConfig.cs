namespace EA.Weee.Web.Maintenance
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery-{version}.js",
                     "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery.select-to-autocomplete.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/custom-validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_toolkit").Include(
                      "~/Scripts/govuk_toolkit/vendor/polyfills/bind.js",
                      "~/Scripts/govuk_toolkit/govuk/selection-buttons.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_elements").Include(
                     "~/Scripts/govuk_elements/vendor/polyfills/bind.js",
                     "~/Scripts/govuk_elements/application.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_weee").Include(
                      "~/Scripts/vendor/modernizr.custom.77028.js",
                      "~/Scripts/vendor/details.polyfill.js",
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
