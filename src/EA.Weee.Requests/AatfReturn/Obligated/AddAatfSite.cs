namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class AddAatfSite : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }
    
        public SiteAddressData AddressData { get; set; }
    }
}