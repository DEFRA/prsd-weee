namespace EA.Weee.RequestHandlers.Mappings
{
    using Domain;
    using Prsd.Core.Mapper;
    using Requests.Organisations.Create;

    public class CreatePartnershipRequestMap : IMap<CreatePartnershipRequest, Organisation>
    {
        public Organisation Map(CreatePartnershipRequest source)
        {
            return new Organisation(OrganisationType.Partnership, OrganisationStatus.Incomplete)
            {
                TradingName = source.TradingName
            };
        }
    }
}
