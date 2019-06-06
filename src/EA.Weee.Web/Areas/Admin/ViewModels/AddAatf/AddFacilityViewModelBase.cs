namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;

    public abstract class AddFacilityViewModelBase : FacilityViewModelBase
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        public AatfContactData ContactData { get; set; }

        public IEnumerable<short> ComplianceYearList => new List<short> { 2019 };

        public AddFacilityViewModelBase()
        {
            ContactData = new AatfContactData();
            SiteAddressData = new AatfAddressData();
        }
    }
}