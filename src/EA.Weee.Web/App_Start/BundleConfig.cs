﻿namespace EA.Weee.Web
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