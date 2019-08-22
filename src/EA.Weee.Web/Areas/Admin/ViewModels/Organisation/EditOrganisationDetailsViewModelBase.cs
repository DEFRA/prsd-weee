namespace EA.Weee.Web.Areas.Admin.ViewModels.Organisation
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using System;

    public abstract class EditOrganisationDetailsViewModelBase
    {
        public Guid? SchemeId { get; set; }

        public Guid OrgId { get; set; }

        public Guid? AatfId { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public FacilityType FacilityType { get; set; }

        public abstract string BusinessTradingName { get; set; }

        public Core.Shared.AddressData BusinessAddress { get; set; }
    }
}