namespace EA.Weee.Requests.Producers
{
    using System;
    using Shared;

    public class ProducerData
    {
        public Guid Id { get; set; }

        public bool IsSiteOfExport { get; set; }

        public BusinessData Business { get; set; }

        public AddressData Address { get; set; }

        public ContactData Contact { get; set; }

        public Guid NotificationId { get; set; }
    }
}