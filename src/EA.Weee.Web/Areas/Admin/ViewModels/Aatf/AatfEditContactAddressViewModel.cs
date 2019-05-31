namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using System;
    using EA.Weee.Core.AatfReturn;

    public class AatfEditContactAddressViewModel
    {
        public Guid AatfId { get; set; }

        public AatfContactData ContactData { get; set; }

        public FacilityType FacilityType { get; set; }

        public AatfEditContactAddressViewModel()
        {
            this.ContactData = new AatfContactData();
        }
    }
}