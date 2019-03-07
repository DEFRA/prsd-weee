namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;
    public class GetReturnScheme : IRequest<SchemeDataList>
    {
        public Guid ReturnId { get; set; }

        public Guid SchemeId { get; set; }

        public GetReturnScheme(Guid returnId)
        {
            this.ReturnId = returnId;
        }
    }
}
