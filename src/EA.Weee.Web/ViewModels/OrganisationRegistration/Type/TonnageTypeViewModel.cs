﻿namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Type
{
    using Core.Organisations;
    using Shared;
    using System.ComponentModel.DataAnnotations;

    public class TonnageTypeViewModel : RadioButtonStringCollectionViewModel
    {
        public string SearchedText { get; set; }

        [Required(ErrorMessage = "Please indicate if you are a producer of electrical or electronic equipment")]
        public override string SelectedValue { get; set; }

        public bool NpwdMigrated { get; set; }

        public TonnageTypeViewModel()
            : base(CreateFromEnum<TonnageType>().PossibleValues)
        {
        }

        public TonnageTypeViewModel(string searchText) : this()
        {
            SearchedText = searchText;
        }

        public TonnageTypeViewModel(TonnageType tonnageType)
        {
            SelectedValue = CreateFromEnum(tonnageType).SelectedValue;
        }
    }
}