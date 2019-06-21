namespace EA.Weee.RequestHandlers.Mappings
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;

    public class AatfContactMap : IMap<AatfContact, AatfContactData>
    {
        public AatfContactData Map(AatfContact source)
        {
            if (source != null)
            {
                var addressData = new AatfContactAddressData(null, source.Address1, source.Address2, source.TownOrCity, source.CountyOrRegion, source.Postcode, source.Country.Id, source.Country.Name);

                return new AatfContactData(
                source.Id,
                source.FirstName,
                source.LastName,
                source.Position,
                addressData,
                source.Telephone,
                source.Email);
            }
            else
            {
                return new AatfContactData();
            }
        }
    }
}
