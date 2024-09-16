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

        public Core.Shared.ServiceOfNoticeAddressData Address { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }
    }
}