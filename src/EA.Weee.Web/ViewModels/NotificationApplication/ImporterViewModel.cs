namespace EA.Weee.Web.ViewModels.NotificationApplication
{
    using System;
    using Shared;

    public class ImporterViewModel
    {
        public Guid NotificationId { get; set; }

        public BusinessViewModel BusinessViewModel { get; set; }

        public AddressViewModel AddressDetails { get; set; }

        public ContactPersonViewModel ContactDetails { get; set; }
    }
}