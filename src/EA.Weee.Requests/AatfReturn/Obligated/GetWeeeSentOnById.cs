namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System;

    public class GetWeeeSentOnById : IRequest<WeeeSentOnData>
    {
        public Guid WeeeSentOnId { get; set; }

        public GetWeeeSentOnById(Guid weeeSentOnId)
        {
            WeeeSentOnId = weeeSentOnId;
        }
    }
}
