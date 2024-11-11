namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using System.Web.UI.WebControls;

    public class ServiceOfNoticeMap : IMap<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>
    {
        private readonly IMapper mapper;

        public ServiceOfNoticeMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ServiceOfNoticeViewModel Map(SmallProducerSubmissionMapperData source)
        {
            var submissionData = source.SmallProducerSubmissionData;

            var viewModel = new ServiceOfNoticeViewModel()
            {
                DirectRegistrantId = submissionData.DirectRegistrantId,
                OrganisationId = submissionData.OrganisationData.Id,
                Address = source.UseMasterVersion ? MapAddress(submissionData.ServiceOfNoticeData) : 
                    (submissionData.CurrentSubmission != null ? MapAddress(submissionData.CurrentSubmission.ServiceOfNoticeData) : new ServiceOfNoticeAddressData()),
                HasAuthorisedRepresentitive = submissionData.HasAuthorisedRepresentitive,
                RedirectToCheckAnswers = source.RedirectToCheckAnswers
            };

            if (viewModel.Address == null)
            {
                viewModel.Address = new ServiceOfNoticeAddressData();
            }

            return viewModel;
        }

        private ServiceOfNoticeAddressData MapAddress(AddressData source) 
        {
            return mapper.Map<AddressData, ServiceOfNoticeAddressData>(source);
        }
    }
}