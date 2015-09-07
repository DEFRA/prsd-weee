namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Organisations;
    using Prsd.Core.Mediator;

    public class GetPublicOrganisationInfo : IRequest<PublicOrganisationData>
    {
        public readonly Guid Id;

        public GetPublicOrganisationInfo(Guid id)
        {
            Id = id;
        }
    }
}