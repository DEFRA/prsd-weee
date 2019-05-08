namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using System;

    public class RemoveAatfSite : IRequest<bool>
    {
        public Guid SiteId { get; set; }

        public Guid AatfId { get; set; }

        public Guid ReturnId { get; set; }

        public RemoveAatfSite(Guid siteId)
        {
            SiteId = siteId;
        }
    }
}
