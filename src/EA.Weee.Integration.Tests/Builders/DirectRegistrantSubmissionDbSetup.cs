namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using System.Globalization;

    public class DirectRegistrantSubmissionDbSetup : DbTestDataBuilder<DirectProducerSubmission, DirectRegistrantSubmissionDbSetup>
    {
        protected override DirectProducerSubmission Instantiate()
        {
            var registeredProducer = new RegisteredProducer(SystemTime.UtcNow.ToString(CultureInfo.InvariantCulture), SystemTime.UtcNow.Year);

            instance = new DirectProducerSubmission(registeredProducer, SystemTime.UtcNow.Year);

            return instance;
        }

        public DirectRegistrantSubmissionDbSetup WithDirectRegistrant(DirectRegistrant directRegistrant)
        {
            //var submissionHistory = new DirectProducerSubmissionHistory(instance, null, null, null);

            ObjectInstantiator<DirectProducerSubmission>.SetProperty(o => o.DirectRegistrant, directRegistrant, instance);
            
            //instance.SubmissionHistory.Add(submissionHistory);

            return this;
        }

        public DirectRegistrantSubmissionDbSetup WithSubmission(DirectProducerSubmissionHistory submissionHistory)
        {
            instance.SubmissionHistory.Add(submissionHistory);

            return this;
        }
    }
}
