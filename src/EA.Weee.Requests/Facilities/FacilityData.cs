namespace EA.Weee.Requests.Facilities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class FacilityData
    {
        public Guid? Id { get; set; }

        public BusinessData Business { get; set; }

        public AddressData Address { get; set; }

        public ContactData Contact { get; set; }

        public Guid NotificationId { get; set; }

        [Display(Name = "Actual site of disposal/recovery")]
        public bool IsActualSiteOfTreatment { get; set; }

        public NotificationType NotificationType { get; set; }

        public FacilityData()
        {
            if (Address == null)
            {
                Address = new AddressData();
            }

            if (Contact == null)
            {
                Contact = new ContactData();
            }

            if (Business == null)
            {
                Business = new BusinessData();
            }
        }
    }
}
