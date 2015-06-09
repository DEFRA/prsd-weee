namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using Prsd.Core.Mapper;
    using Requests.Organisations.Create;

    public class CreateSoleTraderRequestMap : IMap<CreateSoleTraderRequest, Organisation>
    {
        public Organisation Map(CreateSoleTraderRequest source)
        {
            return new Organisation(OrganisationType.SoleTraderOrIndividual, OrganisationStatus.Incomplete)
            {
                TradingName = source.TradingName
            };
        }
    }
}
