﻿namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetAatfById : IRequest<AatfData>
    {
        public Guid AatfId { get; set; }

        public GetAatfById(Guid id)
        {
            AatfId = id;
        }
    }
}
