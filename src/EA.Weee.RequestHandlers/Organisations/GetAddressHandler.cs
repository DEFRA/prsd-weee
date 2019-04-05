namespace EA.Weee.RequestHandlers.Organisations
{
    using System;
    using System.Threading.Tasks;
    using AatfReturn;
    using Core.Shared;
    using DataAccess;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;

    public class GetAddressHandler : IRequestHandler<GetAddress, AddressData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<Address, AddressData> mapper;
        private readonly WeeeContext context;
        
        public GetAddressHandler(IWeeeAuthorization authorization, WeeeContext context, IGenericDataAccess dataAccess, IMap<Address, AddressData> mapper)
        {
            this.authorization = authorization;
            this.context = context;
            this.dataAccess = dataAccess;
            this.mapper = mapper;
        }

        public async Task<AddressData> HandleAsync(GetAddress message)
        {
            authorization.EnsureOrganisationAccess(message.OrganisationId);

            var address = await dataAccess.GetById<Address>(message.AddressId);

            if (address == null)
            {
                throw new ArgumentException($"Could not find an address with Id {message.AddressId}");
            }

            return mapper.Map(address);
        }
    }
}
