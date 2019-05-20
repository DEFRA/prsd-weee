namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;

    public class AatfDataToAatfDetailsViewModelMapTransfer
    {
        public AatfDataToAatfDetailsViewModelMapTransfer(AatfData aatfData, AatfContactData contactData, OrganisationData organisationData)
        {
            AatfData = aatfData;
            ContactData = contactData;
            OrganisationData = organisationData;
        }

        public AatfData AatfData { get; set; }

        public AatfContactData ContactData { get; set; }

        public OrganisationData OrganisationData { get; set; }

        public string OrganisationString { get; set; }
    }
}