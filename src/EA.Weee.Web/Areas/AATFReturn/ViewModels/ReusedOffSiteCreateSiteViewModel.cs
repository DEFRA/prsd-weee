namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using AddressData = Core.AatfReturn.AddressData;

    public class ReusedOffSiteCreateSiteViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid WeeeReusedId { get; set; }

        public AddressData AddressData { get; set; }
    }
}