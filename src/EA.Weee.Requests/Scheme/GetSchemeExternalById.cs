namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetSchemeExternalById : IRequest<SchemeData>
    {
        public Guid SchemeId { get; private set; }

        public GetSchemeExternalById(Guid schemeId)
        {
            SchemeId = schemeId;
        }
    }
}
