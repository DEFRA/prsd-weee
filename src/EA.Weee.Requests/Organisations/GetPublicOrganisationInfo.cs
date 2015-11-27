namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using Prsd.Core.Mediator;
    using System;

    public class GetPublicOrganisationInfo : IRequest<PublicOrganisationData>
    {
        public readonly Guid Id;

        public GetPublicOrganisationInfo(Guid id)
        {
            Id = id;
        }
    }
}