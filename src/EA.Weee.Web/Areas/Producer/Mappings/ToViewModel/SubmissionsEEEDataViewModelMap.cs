namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class SubmissionsEEEDataViewModelMap : IMap<SubmissionsYearDetails, EditEeeDataViewModel>
    {
        private readonly IMapper mapper;

        public SubmissionsEEEDataViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditEeeDataViewModel Map(SubmissionsYearDetails source)
        {
            if (source.Year.HasValue)
            {
                var sub = source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value];

                var editEeeDataViewModel = new EditEeeDataViewModel()
                {
                    OrganisationId = source.SmallProducerSubmissionData.OrganisationData.Id,
                    DirectRegistrantId = source.SmallProducerSubmissionData.DirectRegistrantId,
                    SellingTechnique = SellingTechniqueViewModel.FromSellingTechniqueType(sub.SellingTechnique),
                    HasAuthorisedRepresentitive = source.SmallProducerSubmissionData.HasAuthorisedRepresentitive,
                };

                foreach (var eee in sub.TonnageData)
                {
                    foreach (var producerSubmissionCategoryValue in editEeeDataViewModel.CategoryValues)
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

                return editEeeDataViewModel;
            }

            var sourceMapperData = new SmallProducerSubmissionMapperData()
            {
                SmallProducerSubmissionData = source.SmallProducerSubmissionData
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, EditEeeDataViewModel>(sourceMapperData);

            return model;
        }

        private ServiceOfNoticeAddressData MapAddress(AddressData source)
        {
            return mapper.Map<AddressData, ServiceOfNoticeAddressData>(source);
        }
    }
}