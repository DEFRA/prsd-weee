namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.Organisations;
    using System;

    public class SmallProducerSubmissionData
    {
        public OrganisationData OrganisationData { get; set; }

        public SmallProducerSubmissionHistoryData CurrentSubmission { get; set; }

        public Guid DirectRegistrantId { get; set; }
    }
}