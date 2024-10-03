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
                    Address = new RepresentingCompanyAddressData
                    {
                        Address1 = submission.AuthorisedRepresentitiveData.Address1,
                        Address2 = submission.AuthorisedRepresentitiveData.Address2,
                        CountryId = submission.AuthorisedRepresentitiveData.CountryId,
                        CountyOrRegion = submission.AuthorisedRepresentitiveData.CountyOrRegion,
                        Email = submission.AuthorisedRepresentitiveData.Email,
                        Postcode = submission.AuthorisedRepresentitiveData.Postcode,
                        Telephone = submission.AuthorisedRepresentitiveData.Telephone,
                        TownOrCity = submission.AuthorisedRepresentitiveData.TownOrCity,
                        CountryName = submission.AuthorisedRepresentitiveData.CountryName
                    },
                    BusinessTradingName = submission.AuthorisedRepresentitiveData.BusinessTradingName,
                    CompanyName = submission.AuthorisedRepresentitiveData.CompanyName,
                };
            }

            var modelMapperData = new SmallProducerSubmissionMapperData()
            {
                SmallProducerSubmissionData = source.SmallProducerSubmissionData
            };

            return mapper.Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(modelMapperData);
        }
    }
}