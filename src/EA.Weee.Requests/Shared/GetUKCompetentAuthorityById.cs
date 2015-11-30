namespace EA.Weee.Requests.Shared
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetUKCompetentAuthorityById : IRequest<UKCompetentAuthorityData>
    {
        public Guid Id { get; private set; }

        public GetUKCompetentAuthorityById(Guid id)
        {
            Id = id;
        }
    }
}
