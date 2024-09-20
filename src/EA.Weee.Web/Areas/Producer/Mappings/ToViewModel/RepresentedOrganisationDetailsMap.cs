namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;

    public class RepresentedOrganisationDetailsMap : IMap<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>
    {
        public RepresentingCompanyDetailsViewModel Map(SmallProducerSubmissionMapperData source)
        {
            var submissionData = source.SmallProducerSubmissionData;

            return new RepresentingCompanyDetailsViewModel()
            {
                DirectRegistrantId = submissionData.DirectRegistrantId,
                OrganisationId = submissionData.OrganisationData.Id,
                Address = new RepresentingCompanyAddressData
                {
                    Address1 = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.Address1,
                    Address2 = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.Address2,
                    CountryId = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.CountryId,
                    CountyOrRegion = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.CountyOrRegion,
                    Email = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.Email,
                    Postcode = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.Postcode,
                    Telephone = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.Telephone,
                    TownOrCity = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.TownOrCity,
                    CountryName = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.CountryName
                },
                BusinessTradingName = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.BusinessTradingName,
                CompanyName = submissionData.CurrentSubmission.AuthorisedRepresentitiveData.CompanyName,
                RedirectToCheckAnswers = source.RedirectToCheckAnswers
            };
        }
    }
}