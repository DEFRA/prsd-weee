namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using System;

    public class RegisteredResultModel
    {
        public Guid OrganisationId { get; set; }

        public string PaymentReference { get; set; }

        public int ComplianceYear { get; set; }

        public decimal TotalAmount { get; set; }

        public string ProducerRegistrationNumber { get; set; }

        public string ProducerName { get; set; }

        public bool HasPaid { get; set; }
    }
}