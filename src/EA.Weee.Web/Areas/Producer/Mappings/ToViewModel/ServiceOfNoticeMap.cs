namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;

    public class ServiceOfNoticeMap : IMap<SmallProducerSubmissionData, ServiceOfNoticeViewModel>
    {
        private readonly IMapper mapper;

        public ServiceOfNoticeMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ServiceOfNoticeViewModel Map(SmallProducerSubmissionData source)
        {
            var address = MapAddress(source);

            var viewModel = new ServiceOfNoticeViewModel()
            {
                DirectRegistrantId = source.DirectRegistrantId,
                OrganisationId = source.OrganisationData.Id,
                Address = address,
            };

            return viewModel;
        }

        private AddressPostcodeRequiredData MapAddress(SmallProducerSubmissionData source)
        {
            return mapper.Map<AddressData, AddressPostcodeRequiredData>(source.CurrentSubmission.ServiceOfNoticeData);
        }
    }
}