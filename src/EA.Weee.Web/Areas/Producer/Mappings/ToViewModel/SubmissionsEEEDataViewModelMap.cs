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
            var sourceMapperData = new SmallProducerSubmissionMapperData()
            {
                SmallProducerSubmissionData = source.SmallProducerSubmissionData,
                UseMasterVersion = false,
                Year = source.Year
            };

            var model = mapper.Map<SmallProducerSubmissionMapperData, EditEeeDataViewModel>(sourceMapperData);

            return model;
        }
    }
}