namespace EA.Weee.Requests.Scheme
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System;

    public class GetSchemeById : IRequest<SchemeData>
    {
        public Guid SchemeId { get; private set; }

        public GetSchemeById(Guid schemeId)
        {
            SchemeId = schemeId;
        }
    }
}
