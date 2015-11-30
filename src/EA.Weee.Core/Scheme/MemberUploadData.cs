namespace EA.Weee.Core.Scheme
{
    using System;

    public class MemberUploadData
    {
        public Guid Id { get; set; }

        public int? ComplianceYear { get; set; }

        public bool IsSubmitted { get; set; }

        public Guid? SchemeId { get; set; }

        public Guid OrganisationId { get; set; }

        public decimal TotalCharges { get; set; }
    }
}
