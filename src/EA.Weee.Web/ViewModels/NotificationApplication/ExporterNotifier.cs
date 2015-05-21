namespace EA.Weee.Web.ViewModels.NotificationApplication
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Infrastructure;
    using Prsd.Core.Web;
    using Shared;

    public class ExporterNotifier
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExporterId { get; set; }

        public Guid NotificationId { get; set; }

        public BusinessViewModel BusinessViewModel { get; set; }

        public AddressViewModel AddressDetails { get; set; }

        public ContactPersonViewModel ContactDetails { get; set; }
    }
}