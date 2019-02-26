namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using System;
    using AddressData = Core.AatfReturn.AddressData;

    public class ReusedOffSiteCreateViewModel
    {
        public Guid ReturnId { get; set; }

        public string Name { get; set; }

        public AddressData AddressData { get; set; }
    }
}