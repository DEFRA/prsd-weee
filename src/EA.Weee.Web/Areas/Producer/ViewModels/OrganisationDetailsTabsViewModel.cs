namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using iText.Layout.Element;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationDetailsTabsViewModel
    {
        [DisplayName("Select registration year")]
        [Required]
        public int? Year { get; set; } = null;

        public IEnumerable<int> Years { get; set; }

        public OrganisationDetailsDisplayOption ActiveOption { get; set; }

        public SmallProducerSubmissionData SmallProducerSubmissionData { get; set; }

        public OrganisationViewModel OrganisationViewModel { get; set; }
    }
}