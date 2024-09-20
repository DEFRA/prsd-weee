namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using System;

    public class OrganisationDetailsTabsViewModel
    {
        public OrganisationDetailsDisplayOption ActiveOption { get; set; }

        public SmallProducerSubmissionData SmallProducerSubmissionData { get; set; }

        public OrganisationViewModel OrganisationViewModel { get; set; }
    }
}