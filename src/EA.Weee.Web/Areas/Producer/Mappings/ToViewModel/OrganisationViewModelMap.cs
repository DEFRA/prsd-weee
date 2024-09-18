namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;

    public class OrganisationViewModelMap : IMap<SmallProducerSubmissionData, OrganisationViewModel>
    {
        public OrganisationViewModel Map(SmallProducerSubmissionData source)
        {
            return new OrganisationViewModel()
            {
                Address = new Core.Organisations.ExternalAddressData
                {
                    Address1 = source.OrganisationData.BusinessAddress.Address1,
                    Address2 = source.OrganisationData.BusinessAddress.Address2,
                    CountryId = source.OrganisationData.BusinessAddress.CountryId,
                    CountryName = source.OrganisationData.BusinessAddress.CountryName,
                    Postcode = source.OrganisationData.BusinessAddress.Postcode,
                    TownOrCity = source.OrganisationData.BusinessAddress.TownOrCity,
                    CountyOrRegion = source.OrganisationData.BusinessAddress.CountyOrRegion,
                    WebsiteAddress = source.OrganisationData.BusinessAddress.WebAddress
                },
                CompanyName = source.OrganisationData.Name,
                BusinessTradingName = source.OrganisationData.TradingName,
                CompaniesRegistrationNumber = source.OrganisationData.CompanyRegistrationNumber,
                OrganisationType = MapOrganisationType(source.OrganisationData.OrganisationType)
            };
        }

        private ExternalOrganisationType MapOrganisationType(OrganisationType organisationType)
        {
            switch (organisationType)
            {
                case OrganisationType.Partnership:
                    return ExternalOrganisationType.Partnership;
                case OrganisationType.RegisteredCompany:
                    return ExternalOrganisationType.RegisteredCompany;
                case OrganisationType.SoleTraderOrIndividual:
                    return ExternalOrganisationType.SoleTrader;
                default:
                    return ExternalOrganisationType.RegisteredCompany;
            }
        }
    }
}