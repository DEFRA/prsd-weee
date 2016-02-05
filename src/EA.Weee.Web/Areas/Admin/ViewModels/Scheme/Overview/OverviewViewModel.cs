namespace EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview
{
    using System;

    public abstract class OverviewViewModel
    {
        private string schemeName;

        public Guid SchemeId { get; set; }

        public string SchemeName
        {
            get
            {
                if (string.IsNullOrEmpty(schemeName))
                {
                    return "-";
                }

                return schemeName;
            }
            set { schemeName = value; }
        }

        public OverviewDisplayOption ActiveOverviewDisplayOption { get; private set; }

        protected OverviewViewModel(OverviewDisplayOption activeOverviewDisplayOption)
        {
            ActiveOverviewDisplayOption = activeOverviewDisplayOption;
        }
    }
}