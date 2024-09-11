namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using System;

    public class ServiceOfNoticeViewModel
    {
        public Guid OrganisationId { get; set; }

        //public bool SameAsOrganisationAddress { get; set; }
        public Core.Shared.AddressPostcodeRequiredData Address { get; set; }
    }
}