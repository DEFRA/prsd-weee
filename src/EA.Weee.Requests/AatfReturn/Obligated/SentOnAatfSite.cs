namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using System;

    public class SentOnAatfSite : IRequest<Guid>
    {
        public OperatorAddressData OperatorAddressData { get; set; }

        public AatfAddressData SiteAddressData { get; set; }
    }
}
