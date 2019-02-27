namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    using AddressData = Core.AatfReturn.AddressData;

    public class ReusedOffSiteCreateSiteViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public AddressData AddressData { get; set; }
    }
}