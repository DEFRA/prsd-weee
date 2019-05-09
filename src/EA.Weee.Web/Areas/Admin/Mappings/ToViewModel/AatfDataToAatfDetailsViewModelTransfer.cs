namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;

    public class AatfDataToAatfDetailsViewModelTransfer
    {
        public AatfDataToAatfDetailsViewModelTransfer(AatfData aatfData, AatfContactData contactData)
        {
            AatfData = aatfData;
            ContactData = contactData;
        }

        public AatfData AatfData { get; set; }

        public AatfContactData ContactData { get; set; }
    }
}