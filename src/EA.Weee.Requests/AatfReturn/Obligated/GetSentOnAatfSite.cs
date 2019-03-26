namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System;

    public class GetSentOnAatfSite : IRequest<AatfAddressData>
    {
        public Guid WeeeSentOnId { get; set; }

        public GetSentOnAatfSite(Guid id)
        {
            WeeeSentOnId = id;
        }
    }
}
