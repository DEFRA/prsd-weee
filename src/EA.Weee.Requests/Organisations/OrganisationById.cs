namespace EA.Weee.Requests.Organisations
{
    using System;
    using Prsd.Core.Mediator;

    public class OrganisationById : IRequest<OrganisationData>
    {
        public readonly Guid Id;

        public OrganisationById(Guid id)
        {
            Id = id;
        }
    }
}