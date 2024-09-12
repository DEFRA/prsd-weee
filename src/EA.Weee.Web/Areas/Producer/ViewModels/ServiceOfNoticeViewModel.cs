namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using System;
    using System.ComponentModel;

    public class ServiceOfNoticeViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid DirectRegistrantId { get; set; }

        [DisplayName("Same as organisation address")]
        public bool SameAsOrganisationAddress { get; set; }

        public Core.Shared.AddressPostcodeRequiredData Address { get; set; }
    }
}