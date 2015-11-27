namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Type
{
    using Core.Organisations;
    using Shared;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationTypeViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid? OrganisationId { get; set; }

        public string SearchedText { get; set; }

        [Required(ErrorMessage = "Select the type of organisation that you work for")]
        public override string SelectedValue { get; set; }

        public OrganisationTypeViewModel()
            : base(CreateFromEnum<OrganisationType>().PossibleValues)
        {
        }

        public OrganisationTypeViewModel(string searchText) : this()
        {
            SearchedText = searchText;
        }

        public OrganisationTypeViewModel(Guid orgId) : this(string.Empty)
        {
            OrganisationId = orgId;
        }

        public OrganisationTypeViewModel(OrganisationType organisationType, Guid orgId) : this(orgId)
        {
            SelectedValue = CreateFromEnum(organisationType).SelectedValue;
        }
    }
}