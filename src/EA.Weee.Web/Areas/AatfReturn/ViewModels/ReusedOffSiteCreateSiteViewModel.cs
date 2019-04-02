namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using Core.AatfReturn;
    
    public class ReusedOffSiteCreateSiteViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public Guid? SiteId { get; set; }

        public SiteAddressData AddressData { get; set; }

        public bool Edit => (SiteId.HasValue);

        public ReusedOffSiteCreateSiteViewModel()
        {
            this.AddressData = new SiteAddressData();
        }
    }
}