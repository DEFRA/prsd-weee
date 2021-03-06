﻿namespace EA.Weee.Core.Admin
{
    using System;

    public class SubmissionsHistorySearchData
    {
        public Guid SchemeId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid MemberUploadId { get; set; }

        public int Year { get; set; }

        public DateTime DateTime { get; set; }

        public int NoOfWarnings { get; set; }

        public string SubmittedBy { get; set; }

        public decimal TotalCharges { get; set; }

        public string FileName { get; set; }

        public int? NumberOfNewProducers { get; set; }

        public int? NumberOfChangedProducers { get; set; }

        public bool? ProducersChanged { get; set; }
    }
}