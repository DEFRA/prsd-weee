namespace EA.Weee.Core.DirectRegistrant
{
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SmallProducerSubmissionData
    {
        public OrganisationData OrganisationData { get; set; }

        public ContactData ContactData { get; set; }

        public AddressData ContactAddressData { get; set; }

        public IDictionary<int, SmallProducerSubmissionHistoryData> SubmissionHistory {get; set; }

        public bool AnySubmissionsToDisplay => SubmissionHistory.Any();

        public bool AnySubmissions => SubmissionHistory.Any();

        public SmallProducerSubmissionHistoryData CurrentSubmission { get; set; }

        public Guid DirectRegistrantId { get; set; }

        public bool HasAuthorisedRepresentitive { get; set; }

        public AuthorisedRepresentitiveData AuthorisedRepresentitiveData { get; set; }

        public AddressData ServiceOfNoticeData { get; set; }

        public string ProducerRegistrationNumber { get; set; }

        public int CurrentSystemYear { get; set; }

        public string EeeBrandNames { get; set; }
    }
}