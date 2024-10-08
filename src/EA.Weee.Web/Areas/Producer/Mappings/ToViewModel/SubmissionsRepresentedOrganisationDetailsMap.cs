namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;

    public class SubmissionsRepresentedOrganisationDetailsMap : IMap<SubmissionsYearDetails, RepresentingCompanyDetailsViewModel>
    {
        private readonly IMapper mapper;

        public SubmissionsRepresentedOrganisationDetailsMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public RepresentingCompanyDetailsViewModel Map(SubmissionsYearDetails source)
        {
            if (source.Year.HasValue)
            {
                var submission = source.SmallProducerSubmissionData.SubmissionHistory[source.Year.Value];

                return new RepresentingCompanyDetailsViewModel()
                {
                    DirectRegistrantId = source.SmallProducerSubmissionData.DirectRegistrantId,
                    OrganisationId = source.SmallProducerSubmissionData.OrganisationData.Id,
                    BusinessTradingName = submission.AuthorisedRepresentitiveData.BusinessTradingName,
                    CompanyName = submission.AuthorisedRepresentitiveData.CompanyName,
                    Address = mapper.Map<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>(submission.AuthorisedRepresentitiveData)
                };
            }

            var modelMapperData = new SmallProducerSubmissionMapperData()
            {
                UseMasterVersion = true,
                SmallProducerSubmissionData = source.SmallProducerSubmissionData
            };

            return mapper.Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(modelMapperData);
        }
    }
}