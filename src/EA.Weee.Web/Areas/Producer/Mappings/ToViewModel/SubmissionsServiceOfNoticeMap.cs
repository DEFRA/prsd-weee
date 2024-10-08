namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class SubmissionsServiceOfNoticeMap : IMap<SubmissionsYearDetails, ServiceOfNoticeViewModel>
    {
        private readonly IMapper mapper;

        public SubmissionsServiceOfNoticeMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ServiceOfNoticeViewModel Map(SubmissionsYearDetails source)
        {
            if (source.Year.HasValue)
            {
                var sub = source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value];

                var address = MapAddress(sub.ServiceOfNoticeData);

                var viewModel = new ServiceOfNoticeViewModel()
                {
                    DirectRegistrantId = source.SmallProducerSubmissionData.DirectRegistrantId,
                    OrganisationId = source.SmallProducerSubmissionData.OrganisationData.Id,
                    Address = address ?? new ServiceOfNoticeAddressData(),
                    HasAuthorisedRepresentitive = source.SmallProducerSubmissionData.HasAuthorisedRepresentitive,
                };

                return viewModel;
            }

            var sourceMapperData = new SmallProducerSubmissionMapperData
            {
                UseMasterVersion = false,
                SmallProducerSubmissionData = source.SmallProducerSubmissionData
            };

            var serviceOfNoticeViewModel = mapper.Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>(sourceMapperData);

            return serviceOfNoticeViewModel;
        }

        private ServiceOfNoticeAddressData MapAddress(AddressData source)
        {
            return mapper.Map<AddressData, ServiceOfNoticeAddressData>(source);
        }
    }
}