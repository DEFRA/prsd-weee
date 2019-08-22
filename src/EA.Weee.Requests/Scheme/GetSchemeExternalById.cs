namespace EA.Weee.Requests.Scheme
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System;

    public class GetSchemeExternalById : IRequest<SchemeData>
    {
        public Guid SchemeId { get; private set; }

        public GetSchemeExternalById(Guid schemeId)
        {
            SchemeId = schemeId;
        }
    }
}
