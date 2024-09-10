namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.Organisations.Base;
    using System;

    public class EditOrganisationDetailsViewModel
    {
        public OrganisationViewModel Organisation { get; set; }

        public object CastedModel => this.Organisation.CastToSpecificViewModel(this.Organisation);

        public Guid DirectRegistrantId { get; set; }
    }
}