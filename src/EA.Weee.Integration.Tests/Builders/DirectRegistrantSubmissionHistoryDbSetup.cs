namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using System.Globalization;

    public class DirectRegistrantSubmissionHistoryDbSetup : DbTestDataBuilder<DirectProducerSubmissionHistory, DirectRegistrantSubmissionHistoryDbSetup>
    {
        protected override DirectProducerSubmissionHistory Instantiate()
        {
            instance = new DirectProducerSubmissionHistory();

            return instance;
        }

        public DirectRegistrantSubmissionHistoryDbSetup WithDirectProducerSubmission(DirectProducerSubmission submission)
        {
            ObjectInstantiator<DirectProducerSubmissionHistory>.SetProperty(o => o.DirectProducerSubmissionId, submission.Id, instance);

            return this;
        }
    }
}
