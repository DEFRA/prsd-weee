namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;

    public class RepresentedOrganisationDetailsMap : IMap<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>
    {
        private readonly IMapper mapper;

        public RepresentedOrganisationDetailsMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public RepresentingCompanyDetailsViewModel Map(SmallProducerSubmissionMapperData source)
        {
            var submissionData = source.SmallProducerSubmissionData;

            if (!source.UseCurrentVersion)
            {
                return new RepresentingCompanyDetailsViewModel()
                {
                    DirectRegistrantId = submissionData.DirectRegistrantId,
                    OrganisationId = submissionData.OrganisationData.Id,
                    Address = mapper.Map<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>(submissionData.CurrentSubmission.AuthorisedRepresentitiveData),
                    BusinessTradingName = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.BusinessTradingName,
                    CompanyName = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.CompanyName,
                    RedirectToCheckAnswers = source.RedirectToCheckAnswers
                };
            }

            return new RepresentingCompanyDetailsViewModel()
            {
                Address = mapper.Map<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>(submissionData.AuthorisedRepresentitiveData),
            };
        }
    }
}