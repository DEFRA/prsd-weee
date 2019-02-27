namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using EA.Weee.Core.AatfReturn;

    public class AddOffSite
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public WeeeReused WeeeReused { get; set;}
    
        public AddressData AddressData { get; set; }
    }
}
