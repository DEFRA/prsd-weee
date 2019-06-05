namespace EA.Weee.Web.Areas.Admin.ViewModels.AddAatf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataStandards;

    public abstract class AddFacilityViewModelBase : FacilityViewModelBase
    {
        public Guid OrganisationId { get; set; }

        public string OrganisationName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public abstract string Name { get; set; }

        public AatfAddressData SiteAddressData { get; set; }

        public AatfContactData ContactData { get; set; }

        public IEnumerable<short> ComplianceYearList => new List<short> { 2019 };

        public AddFacilityViewModelBase()
        {
            ContactData = new AatfContactData();
            SiteAddressData = new AatfAddressData();
        }
    }
}