﻿namespace EA.Weee.Web.ViewModels.Shared.Submission
{
    using Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ChooseSubmissionTypeViewModel : RadioButtonStringCollectionViewModel
    {
        public Guid OrganisationId { get; set; }

        [Required(ErrorMessage = "Select which type of submission to view")]
        public override string SelectedValue { get; set; }

        public ChooseSubmissionTypeViewModel()
        {
        }

        public ChooseSubmissionTypeViewModel(List<string> activites) : base(activites)
        {
        }
    }
}