namespace EA.Weee.Web.Areas.Producer.Mappings.ToRequest
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Requests.Base;

    public class ToServiceOfNoticeRequestCreator : IRequestCreator<ServiceOfNoticeViewModel, ServiceOfNoticeRequest>
    {
        private readonly IMapper mapper;

        public ToServiceOfNoticeRequestCreator(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ServiceOfNoticeRequest ViewModelToRequest(ServiceOfNoticeViewModel viewModel)
        {
            var address = mapper.Map<AddressPostcodeRequiredData, AddressData>(viewModel.Address);

            return new ServiceOfNoticeRequest(address);
        }
    }
}