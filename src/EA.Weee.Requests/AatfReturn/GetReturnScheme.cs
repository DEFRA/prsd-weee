namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using EA.Prsd.Core.Mediator;
    using System;
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
