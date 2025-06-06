﻿namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using System.Collections.Generic;
    using System.Globalization;

    public class DirectRegistrantSubmissionDbSetup : DbTestDataBuilder<DirectProducerSubmission, DirectRegistrantSubmissionDbSetup>
    {
        protected override DirectProducerSubmission Instantiate()
        {
            instance = new DirectProducerSubmission();

            return instance;
        }

        public DirectRegistrantSubmissionDbSetup WithDefaultRegisteredProducer()
        {
            var registeredProducer = new RegisteredProducer(SystemTime.UtcNow.Ticks.ToString(), SystemTime.UtcNow.Year);

            ObjectInstantiator<DirectProducerSubmission>.SetProperty(o => o.RegisteredProducer, registeredProducer, instance);

            return this;
        }

        public DirectRegistrantSubmissionDbSetup WithDirectRegistrant(DirectRegistrant directRegistrant)
        {
            ObjectInstantiator<DirectProducerSubmission>.SetProperty(o => o.DirectRegistrant, directRegistrant, instance);
            
            return this;
        }

        public DirectRegistrantSubmissionDbSetup WithRegisteredProducer(RegisteredProducer registeredProducer)
        {
            ObjectInstantiator<DirectProducerSubmission>.SetProperty(o => o.RegisteredProducer, registeredProducer, instance);

            return this;
        }

        public DirectRegistrantSubmissionDbSetup WithComplianceYear(int year)
        {
            ObjectInstantiator<DirectProducerSubmission>.SetProperty(o => o.ComplianceYear, year, instance);

            return this;
        }

        public DirectRegistrantSubmissionDbSetup WithStatus(DirectProducerSubmissionStatus status)
        {
            ObjectInstantiator<DirectProducerSubmission>.SetProperty(o => o.DirectProducerSubmissionStatus, status, instance);

            return this;
        }
    }
}
