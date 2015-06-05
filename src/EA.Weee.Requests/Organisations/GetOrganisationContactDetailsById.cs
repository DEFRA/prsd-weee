namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;
    using Shared;

    public class GetOrganisationContactDetailsById : IRequest<AddressData>
    {
        public readonly Guid Id;

        public GetOrganisationContactDetailsById(Guid id)
        {
            Id = id;
        }
    }
}
