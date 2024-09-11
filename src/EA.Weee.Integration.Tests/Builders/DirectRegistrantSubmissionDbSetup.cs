namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using System.Globalization;

    public class DirectRegistrantSubmissionDbSetup : DbTestDataBuilder<DirectProducerSubmission, DirectRegistrantSubmissionDbSetup>
    {
        protected override DirectProducerSubmission Instantiate()
        {
            var directRegistrant = DirectRegistrantDbSetup.Init().Create();
            var registeredProducer = new Domain.Producer.RegisteredProducer(SystemTime.UtcNow.ToString(CultureInfo.InvariantCulture), SystemTime.UtcNow.Year);
            instance = new DirectProducerSubmission(directRegistrant, registeredProducer, SystemTime.UtcNow.Year);

            //return instance;
            return instance;
        }

        public DirectRegistrantSubmissionDbSetup WithDirectRegistrantAndSubmission(DirectRegistrant directRegistrant)
        {
            ObjectInstantiator<DirectProducerSubmission>.SetProperty(o => o.DirectRegistrant, directRegistrant, instance);
            
            return this;
        }
    }
}
