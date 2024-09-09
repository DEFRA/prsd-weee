namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;

    public class SmallProducerSubmissionHistoryData
    {
        public int ComplianceYear { get; set; }

        public bool OrganisationDetailsComplete { get; set; }
        public bool ContactDetailsComplete { get; set; }
        public bool ServiceOfNoticeComplete { get; set; }
        public bool RepresentingCompanyDetailsComplete { get; set; }
        public bool EEEDetailsComplete { get; set; }
    }
}
