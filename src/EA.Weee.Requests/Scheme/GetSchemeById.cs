namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetSchemeById : IRequest<SchemeData>
    {
        public Guid SchemeId { get; private set; }

        public GetSchemeById(Guid schemeId)
        {
            SchemeId = schemeId;
        }
    }
}
