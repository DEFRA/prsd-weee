﻿namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class EditEeeDataViewModelMap : IMap<SmallProducerSubmissionData, EditEeeDataViewModel>
    {
        public EditEeeDataViewModel Map(SmallProducerSubmissionData source)
        {
            var model = new EditEeeDataViewModel()
            {
                OrganisationId = source.OrganisationData.Id,
                DirectRegistrantId = source.DirectRegistrantId
            };

            foreach (var eee in source.CurrentSubmission.TonnageData)
            {
                foreach (var producerSubmissionCategoryValue in model.CategoryValues)
                {
                    if (producerSubmissionCategoryValue.CategoryId.ToInt() == eee.Category.ToInt() && eee.ObligationType == ObligationType.B2C)
                    {
                        producerSubmissionCategoryValue.HouseHold = eee.Tonnage.ToTonnageEditDisplay();
                    }
                    if (producerSubmissionCategoryValue.CategoryId.ToInt() == eee.Category.ToInt() && eee.ObligationType == ObligationType.B2B)
                    {
                        producerSubmissionCategoryValue.NonHouseHold = eee.Tonnage.ToTonnageEditDisplay();
                    }
                }    
            }

            return model;
        }
    }
}