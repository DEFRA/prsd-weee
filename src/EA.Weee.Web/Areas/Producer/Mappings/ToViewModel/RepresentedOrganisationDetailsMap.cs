namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;

    public class RepresentedOrganisationDetailsMap : IMap<SmallProducerSubmissionData, RepresentingCompanyDetailsViewModel>
    {
        public RepresentingCompanyDetailsViewModel Map(SmallProducerSubmissionData source)
        {
            return new RepresentingCompanyDetailsViewModel()
            {
                DirectRegistrantId = source.DirectRegistrantId,
                OrganisationId = source.OrganisationData.Id,
                Address = new RepresentingCompanyAddressData
                {
                    Address1 = source.CurrentSubmission.AuthorisedRepresentitiveData.Address1,
                    Address2 = source.CurrentSubmission.AuthorisedRepresentitiveData.Address2,
                    CountryId = source.CurrentSubmission.AuthorisedRepresentitiveData.CountryId,
                    CountyOrRegion = source.CurrentSubmission.AuthorisedRepresentitiveData.CountyOrRegion,
                    Email = source.CurrentSubmission.AuthorisedRepresentitiveData.Email,
                    Postcode = source.CurrentSubmission.AuthorisedRepresentitiveData.Postcode,
                    Telephone = source.CurrentSubmission.AuthorisedRepresentitiveData.Telephone,
                    TownOrCity = source.CurrentSubmission.AuthorisedRepresentitiveData.TownOrCity,
                },
                BusinessTradingName = source.CurrentSubmission.AuthorisedRepresentitiveData.BusinessTradingName,
                CompanyName = source.CurrentSubmission.AuthorisedRepresentitiveData.CompanyName
            };
        }
    }
}