namespace EA.Weee.Requests.Scheme
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class GetSchemesExternal : IRequest<List<SchemeData>>
    {
        public bool IncludeWithdrawn { get; private set; }

        public GetSchemesExternal(bool includeWithDrawn)
        {
            IncludeWithdrawn = includeWithDrawn;
        }
    }
}
