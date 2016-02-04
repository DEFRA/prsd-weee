namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview
{
    using System;

    public abstract class OverviewViewModel
    {
        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public OverviewDisplayOption ActiveOverviewDisplayOption { get; set; }

        protected OverviewViewModel(Guid schemeId, string schemeName, OverviewDisplayOption activeOverviewDisplayOption)
        {
            SchemeId = schemeId;
            SchemeName = string.IsNullOrEmpty(schemeName)
                ? "-"
                : schemeName;
            ActiveOverviewDisplayOption = activeOverviewDisplayOption;
        }
    }
}