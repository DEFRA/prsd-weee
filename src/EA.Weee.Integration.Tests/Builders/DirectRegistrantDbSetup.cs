namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using System.Globalization;

    public class DirectRegistrantDbSetup : DbTestDataBuilder<DirectRegistrant, DirectRegistrantDbSetup>
    {
        protected override DirectRegistrant Instantiate()
        {
            var organisation = OrganisationDbSetup.Init().Create();

            instance = new DirectRegistrant(organisation);

            return instance;
        }

        public DirectRegistrantDbSetup WithSubmissions()
        {
            var registeredProducer = new RegisteredProducer(SystemTime.UtcNow.ToString(CultureInfo.InvariantCulture), SystemTime.UtcNow.Year);

            var directRegistrantSubmission = new DirectProducerSubmission(instance, registeredProducer, SystemTime.UtcNow.Year);
            var directProducerSubmissionHistory = new DirectProducerSubmissionHistory(directRegistrantSubmission);

            return this;
        }
    }
}
