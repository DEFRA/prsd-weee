namespace EA.Weee.Requests.Notification
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Shared;

    public class ExporterData
    {
        [Display(Name = "Company name")]
        public string Name { get; set; }

        public string Type { get; set; }

        [Display(Name = "Registration number")]
        public string RegistrationNumber { get; set; }

        [Display(Name = "Additional registration number")]
        public string AdditionalRegistrationNumber { get; set; }

        public AddressData Address { get; set; }

        public ContactData Contact { get; set; }

        public Guid NotificationId { get; set; }

        public Guid ExporterId { get; set; }
    }
}
