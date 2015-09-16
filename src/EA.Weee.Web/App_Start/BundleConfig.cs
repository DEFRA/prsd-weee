namespace EA.Weee.Web
{
    using System.Web.Optimization;
    using LibSassNet.Web;

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

            // JQuery 2.x.x is used for allother browsers.
            bundles.Add(new ScriptBundle("~/bundles/jquery-2").Include(
                "~/Scripts/jquery-2*",
                "~/Scripts/jquery.unobtrusive-ajax.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery.select-to-autocomplete.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/custom-validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_toolkit").Include(
                      "~/Scripts/govuk_toolkit/vendor/polyfills/bind.js",
                      "~/Scripts/govuk_toolkit/govuk/selection-buttons.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_weee").Include(
                      "~/Scripts/vendor/modernizr.custom.77028.js",
                      "~/Scripts/vendor/details.polyfill.js",
                      "~/Scripts/application.js"));

            bundles.Add(new SassBundle("~/Content/weee-page-ie6", "~/Content/govuk_toolkit/stylesheets").Include(
                      "~/Content/weee-page-ie6.scss"));

            bundles.Add(new SassBundle("~/Content/weee-page-ie7", "~/Content/govuk_toolkit/stylesheets").Include(
                      "~/Content/weee-page-ie7.scss"));

            bundles.Add(new SassBundle("~/Content/weee-page-ie8", "~/Content/govuk_toolkit/stylesheets").Include(
                      "~/Content/weee-page-ie8.scss"));

            bundles.Add(new SassBundle("~/Content/weee-page", "~/Content/govuk_toolkit/stylesheets").Include(
                      "~/Content/weee-page.scss"));

            bundles.Add(new SassBundle("~/Content/prism", "~/Content/govuk_toolkit/stylesheets").Include(
                      "~/Content/prism.scss"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}