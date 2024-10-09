namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;

    public class RepresentedCompanyAddressMap : IMap<AuthorisedRepresentitiveData, RepresentingCompanyAddressData>
    {
        public RepresentingCompanyAddressData Map(AuthorisedRepresentitiveData source)
        {
            return new RepresentingCompanyAddressData()
            {
                Address1 = source.Address1,
                Address2 = source.Address2,
                CountryId = source.CountryId,
                CountyOrRegion = source.CountyOrRegion,
                Email = source.Email,
                Postcode = source.Postcode,
                Telephone = source.Telephone,
                TownOrCity = source.TownOrCity,
                CountryName = source.CountryName
            };
        }
    }
}