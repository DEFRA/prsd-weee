namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class GetObligatedReused : IRequest<ObligatedData>
    {
        public Guid ReturnId { get; set; }

        public GetObligatedReused(Guid returnId)
        {
            this.ReturnId = returnId;
        }
    }
}
