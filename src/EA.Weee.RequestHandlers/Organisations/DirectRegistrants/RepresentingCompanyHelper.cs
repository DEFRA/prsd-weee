namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Producer;

    internal static class RepresentingCompanyHelper
    {
        public static AuthorisedRepresentative CreateRepresentingCompany(RepresentingCompanyDetailsViewModel representingCompanyDetails, Country country)
        {
            AuthorisedRepresentative authorisedRepresentative = null;

            var producerAddress = new ProducerAddress(
                representingCompanyDetails.Address.Address1,
                string.Empty,
                representingCompanyDetails.Address.Address2 ?? string.Empty,
                representingCompanyDetails.Address.TownOrCity,
                string.Empty,
                representingCompanyDetails.Address.CountyOrRegion ?? string.Empty,
                country,
                representingCompanyDetails.Address.Postcode);

            var producerContact = new ProducerContact(string.Empty,
                string.Empty,
                string.Empty,
                representingCompanyDetails.Address.Telephone,
                string.Empty,
                string.Empty,
                representingCompanyDetails.Address.Email,
                producerAddress);

            authorisedRepresentative = new AuthorisedRepresentative(
                representingCompanyDetails.CompanyName,
                representingCompanyDetails.BusinessTradingName,
                producerContact);

            return authorisedRepresentative;
        }
    }
}
