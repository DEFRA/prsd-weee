namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.Organisations;

    public class SmallProducerSubmissionData
    {
        public OrganisationData OrganisationData { get; set; }

        public SmallProducerSubmissionHistoryData CurrentSubmission { get; set; }
    }
}