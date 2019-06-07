namespace EA.Weee.Web.Areas.Admin.ViewModels.Organisation
{
    using System;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;

    public abstract class EditOrganisationDetailsViewModelBase
    {
        public Guid? SchemeId { get; set; }

        public Guid OrgId { get; set; }

        public Guid? AatfId { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public abstract string BusinessTradingName { get; set; }

        public AddressData BusinessAddress { get; set; }

        public string PreviousController { get; set; }
    }
}