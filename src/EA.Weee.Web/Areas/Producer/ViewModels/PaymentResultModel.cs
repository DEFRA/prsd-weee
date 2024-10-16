﻿namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using System;

    public class PaymentResultModel
    {
        public Guid OrganisationId { get; set; }

        public string PaymentReference { get; set; }

        public int ComplianceYear { get; set; }

        public int TotalAmount { get; set; }
    }
}