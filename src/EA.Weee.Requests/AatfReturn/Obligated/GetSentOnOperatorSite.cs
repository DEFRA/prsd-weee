namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System;

    public class GetSentOnOperatorSite : IRequest<AatfAddressData>
    {
        public Guid WeeeSentOnId { get; set; }

        public GetSentOnOperatorSite(Guid id)
        {
            WeeeSentOnId = id;
        }
    }
}
