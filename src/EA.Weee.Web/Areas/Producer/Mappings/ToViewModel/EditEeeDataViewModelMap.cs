﻿namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class EditEeeDataViewModelMap : IMap<SmallProducerSubmissionMapperData, EditEeeDataViewModel>
    {
        public EditEeeDataViewModel Map(SmallProducerSubmissionMapperData source)
        {
            var submissionData = source.SmallProducerSubmissionData;

            var current = source.SmallProducerSubmissionData.CurrentSubmission;

            var submission = source.Year.HasValue ? source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value] : current;

            var model = new EditEeeDataViewModel()
            {
                OrganisationId = submissionData.OrganisationData.Id,
                DirectRegistrantId = submissionData.DirectRegistrantId,
                SellingTechnique = SellingTechniqueViewModel.FromSellingTechniqueType(submission.SellingTechnique),
                HasAuthorisedRepresentitive = submissionData.HasAuthorisedRepresentitive,
                RedirectToCheckAnswers = source.RedirectToCheckAnswers
            };

            foreach (var eee in submission.TonnageData)
            {
                foreach (var producerSubmissionCategoryValue in model.CategoryValues)
                {
                    if (producerSubmissionCategoryValue.CategoryId.ToInt() == eee.Category.ToInt() && eee.ObligationType == Core.Shared.ObligationType.B2C)
                    {
                        producerSubmissionCategoryValue.HouseHold = eee.Tonnage.ToTonnageEditDisplay();
                    }
                    if (producerSubmissionCategoryValue.CategoryId.ToInt() == eee.Category.ToInt() && eee.ObligationType == Core.Shared.ObligationType.B2B)
                    {
                        producerSubmissionCategoryValue.NonHouseHold = eee.Tonnage.ToTonnageEditDisplay();
                    }
                }    
            }

            return model;
        }
    }
}