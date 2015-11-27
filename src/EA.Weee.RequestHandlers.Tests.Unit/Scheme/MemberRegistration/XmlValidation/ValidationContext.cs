namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation
{
    using DataAccess;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using System.Collections.Generic;
    using Weee.Tests.Core;

    public class ValidationContext
    {
        private readonly WeeeContext context;

        private ValidationContext(Scheme scheme)
        {
            context = A.Fake<WeeeContext>();
            var contextHelper = new DbContextHelper();
            var schemesDbSet = contextHelper.GetAsyncEnabledDbSet(new List<Scheme>
            {
               scheme
            });
            A.CallTo(() => context.Schemes).Returns(schemesDbSet);
            A.CallTo(() => context.Producers).Returns(contextHelper.GetAsyncEnabledDbSet(scheme.Producers));
            A.CallTo(() => context.MigratedProducers)
                .Returns(contextHelper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
        }

        private ValidationContext(IEnumerable<Producer> producers, IEnumerable<MigratedProducer> migratedProducers)
        {
            context = A.Fake<WeeeContext>();
            var contextHelper = new DbContextHelper();

            A.CallTo(() => context.Producers).Returns(contextHelper.GetAsyncEnabledDbSet(producers));
            A.CallTo(() => context.MigratedProducers).Returns(contextHelper.GetAsyncEnabledDbSet(migratedProducers));
        }

        public static WeeeContext Create(IEnumerable<Producer> producers, IEnumerable<MigratedProducer> migratedProducers)
        {
            var validationContext = new ValidationContext(producers, migratedProducers);
            return validationContext.context;
        }

        public static WeeeContext Create(Scheme scheme)
        {
            var validationContext = new ValidationContext(scheme);
            return validationContext.context;
        }
    }
}
