namespace EA.Weee.RequestHandlers.Organisations
{
    using AatfReturn;
    using Core.Shared;
    using Domain.Organisation;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Security;
    using System;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class GetAddressHandler : IRequestHandler<GetAddress, AddressData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<Address, AddressData> mapper;

        public GetAddressHandler(IWeeeAuthorization authorization, IGenericDataAccess dataAccess, IMap<Address, AddressData> mapper)
        {
            this.authorization = authorization;
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
