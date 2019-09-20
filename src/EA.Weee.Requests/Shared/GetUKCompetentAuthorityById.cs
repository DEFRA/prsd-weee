namespace EA.Weee.Requests.Shared
{
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class GetUKCompetentAuthorityById : IRequest<UKCompetentAuthorityData>
    {
        public Guid Id { get; private set; }

        public GetUKCompetentAuthorityById(Guid id)
        {
            Id = id;
        }
    }
}
