namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class AddOffSite : IRequest<bool>
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid WeeeReusedId { get; set; }
    
        public AddressData AddressData { get; set; }
    }
}