namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.Organisations;
    using System;
    using System.Collections.Generic;

    public class SmallProducerSubmissionData
    {
        public OrganisationData OrganisationData { get; set; }

        public ContactData ContactData { get; set; }

        public IDictionary<int, SmallProducerSubmissionHistoryData> SubmissionHistory {get; set; }

        public SmallProducerSubmissionHistoryData CurrentSubmission { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }
    }
}