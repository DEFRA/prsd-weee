﻿namespace EA.Weee.Web
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // JQuery 3.x.x is used for all other browsers.
            bundles.Add(new ScriptBundle("~/bundles/jquery-3").Include(
                    "~/Scripts/jquery-3.6.0.js",
                    "~/Scripts/jquery-migrate-3.0.0.js",
                    "~/Scripts/jquery.unobtrusive-ajax.js",
                    "~/Scripts/jquery-ui-1-13.3.js",
                    "~/Scripts/jquery.select-to-autocomplete.js",
                    "~/Scripts/jquery.validate*",
                    "~/Scripts/custom-validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_frontend").Include(
                    "~/Scripts/govuk_frontend/govuk-frontend-4.0.1.min.js",
                    "~/Scripts/govuk_frontend/govuk-frontend-legacy-cookie.js",
                    "~/Scripts/setup-govuk-frontend.js"));

            bundles.Add(new ScriptBundle("~/bundles/govuk_weee").Include(
                    "~/Scripts/vendor/modernizr.custom.77028.js",
                    "~/Scripts/flatpickr.js",
                    "~/Scripts/flatpickr.uk.js",
                    "~/Scripts/auto-complete.min.js",
                    "~/Scripts/weee-application.js",
                    "~/Scripts/jquery-ui-date-picker.js"));

            bundles.Add(new ScriptBundle("~/bundles/weee_tonnage_totals").Include(
                "~/Scripts/weee-tonnage-totals.js"));

            bundles.Add(new ScriptBundle("~/bundles/contact-details-address-lookup")
                .Include("~/Scripts/lookup/lookup-search.js")
                .Include("~/Scripts/lookup/contact-details-address-lookup.js"));

            bundles.Add(new ScriptBundle("~/bundles/org-details-company-lookup")
               .Include("~/Scripts/lookup/lookup-search.js")
               .Include("~/Scripts/lookup/org-details-company-lookup.js"));

            bundles.Add(new ScriptBundle("~/bundles/sent-on-operator-address")
                .Include("~/Scripts/sent-on-operator-address.js"));

            bundles.Add(new ScriptBundle("~/bundles/weee-collapsible-link").Include(
                "~/Scripts/weee-collapsible-link.js"));

            bundles.Add(new ScriptBundle("~/bundles/weee-checkbox-toggle-visibility").Include(
                "~/Scripts/weee-checkbox-toggle-visibility.js"));

            bundles.Add(new ScriptBundle("~/bundles/hide-classes-when-competent-authory-is-not-ea").Include(
                "~/Scripts/hide-classes-when-competent-authory-is-not-ea.js"));

            bundles.Add(new ScriptBundle("~/bundles/show-warning").Include(
                "~/Scripts/show-warning.js"));

            bundles.Add(new ScriptBundle("~/bundles/search-an-aatf").Include(
                "~/Scripts/search-an-aatf.js"));

            bundles.Add(new StyleBundle("~/Content/weee-page-ie6").Include(
                      "~/Content/weee-page-ie6.css"));

            bundles.Add(new StyleBundle("~/Content/weee-page-ie7").Include(
                      "~/Content/weee-page-ie7.css"));

            bundles.Add(new StyleBundle("~/Content/weee-page-ie8").Include(
                      "~/Content/weee-page-ie8.css"));

            bundles.Add(new StyleBundle("~/Content/weee-page").Include(
                            "~/Content/flatpickr.css",
                            "~/Content/weee-page.css",
                            "~/Content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/jquery")
                .Include("~/Content/themes/base/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/remove-site").Include(
                "~/Content/remove-site.css"));

            bundles.Add(new StyleBundle("~/Content/weee-returns-aatf-mobile").Include(
                "~/Content/weee-returns-aatf-mobile.css"));

            bundles.Add(new StyleBundle("~/Content/weee-returns-ae-mobile").Include(
                "~/Content/weee-returns-ae-mobile.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}