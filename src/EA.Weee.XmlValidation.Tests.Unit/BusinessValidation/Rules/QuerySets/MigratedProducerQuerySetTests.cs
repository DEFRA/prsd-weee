namespace EA.Weee.XmlValidation.Tests.Unit.BusinessValidation.Rules.QuerySets
{
    using System.Collections.Generic;
    using DataAccess;
    using FakeItEasy;
    using Weee.Domain.Producer;
    using Weee.Tests.Core;
    using XmlValidation.BusinessValidation.QuerySets;
    using Xunit;

    public class MigratedProducerQuerySetTests
    {
        private readonly WeeeContext context;
        private readonly DbContextHelper helper;

        public MigratedProducerQuerySetTests()
        {
            context = A.Fake<WeeeContext>();
            helper = new DbContextHelper();

            // By default, context returns no migrated producers
            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>()));
        }

        [Theory]
        [InlineData("ABC12345", "ABC12346")]
        [InlineData("ABC12346", "ABC12345")]
        public void GetMigratedProducer_PrnDoesNotMatch_ReturnsNull(string thisPrn, string existingPrn)
        {
            var migratedProducer = FakeMigratedProducer(existingPrn);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>
                {
                    migratedProducer
                }));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>()));

            var result = MigratedProducerQuerySet().GetMigratedProducer(thisPrn);

            Assert.Null(result);
        }

        [Fact]
        public void GetMigratedProducer_PrnMatches_ReturnsMigratedProducer()
        {
            const string prn = "ABC12345";

            var migratedProducer = FakeMigratedProducer(prn);

            A.CallTo(() => context.MigratedProducers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<MigratedProducer>
                {
                    migratedProducer
                }));
            A.CallTo(() => context.Producers)
                .Returns(helper.GetAsyncEnabledDbSet(new List<Producer>()));

            var result = MigratedProducerQuerySet().GetMigratedProducer(prn);

            Assert.Equal(migratedProducer, result);
        }

        private MigratedProducerQuerySet MigratedProducerQuerySet()
        {
            return new MigratedProducerQuerySet(context);
        }

        private MigratedProducer FakeMigratedProducer(string prn)
        {
            var producer = A.Fake<MigratedProducer>();
            A.CallTo(() => producer.ProducerRegistrationNumber)
                .Returns(prn);

            return producer;
        }
    }
}
