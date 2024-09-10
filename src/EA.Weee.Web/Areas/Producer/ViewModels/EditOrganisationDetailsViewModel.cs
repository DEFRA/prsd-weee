namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.Organisations.Base;
    using System;

    public class EditOrganisationDetailsViewModel
    {
        public OrganisationViewModel Organisation { get; set; }
        //public OrganisationViewModel Organisation
        //{
        //    get => organisation.CastToSpecificViewModel(organisation);
        //    set => organisation = (OrganisationViewModel)value;
        //}

        //public object CastedModel => this.Organisation.CastToSpecificViewModel(this.Organisation);

        public Guid DirectRegistrantId { get; set; }
    }
}