namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class GetReturnScheme : IRequest<List<SchemeData>>
    {
        public Guid ReturnId { get; set; }

        public Guid SchemeId { get; set; }

        public GetReturnScheme(Guid returnId)
        {
            this.ReturnId = returnId;
        }
    }
}
