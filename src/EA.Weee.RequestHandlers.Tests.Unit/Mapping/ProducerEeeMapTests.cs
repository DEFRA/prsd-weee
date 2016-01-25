namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Producer;
    using Domain.Scheme;
    using Mappings;
    using Xunit;

    public class ProducerEeeMapTests
    {
        [Fact]
        public void NoProducerEeeByQuarter_ReturnsEmptyEeeData()
        {
            var map = new ProducerEeeMap();

            var result = map.Map(new List<ProducerEeeByQuarter>());

            Assert.Empty(result.TotalEee);
        }

        [Fact]
        public void FirstQuarterHasEeeDataForOneCategory_Q1ContainsTonnage_AndTotalEeeContainsTonnage()
        {
            var map = new ProducerEeeMap();

            var scheme = new Scheme(Guid.NewGuid());
            var producer = new RegisteredProducer("ABC12345", 2016, scheme);
            const decimal tonnage = 10;

            var data = new List<ProducerEeeByQuarter>
            {
                new ProducerEeeByQuarter
                {
                    Eee = new List<EeeOutputAmount>
                    {
                        new EeeOutputAmount(ObligationType.B2B, WeeeCategory.AutomaticDispensers, tonnage, producer)
                    },
                    Quarter = new Quarter(2016, QuarterType.Q1)
                }
            };

            var result = map.Map(data);

            Assert.NotEmpty(result.Q1EEE);
            Assert.Equal(tonnage, result.Q1EEE.Sum(eee => eee.Tonnage));

            Assert.NotEmpty(result.TotalEee);
            Assert.Equal(tonnage, result.TotalEee.Sum(eee => eee.Tonnage));
        }

        [Fact]
        public void FirstAndSecondQuarterHasEeeDataForTheSameCategoryAndObligationType_Q1ContainsTonnage_AndQ2ContainsTonnage_AndTotalEeeContainsSumOfTonnage()
        {
            var map = new ProducerEeeMap();

            var scheme = new Scheme(Guid.NewGuid());
            var producer = new RegisteredProducer("ABC12345", 2016, scheme);
            const decimal tonnageForQ1 = 10M;
            const decimal tonnageForQ2 = 25.6M;

            var data = new List<ProducerEeeByQuarter>
            {
                new ProducerEeeByQuarter
                {
                    Eee = new List<EeeOutputAmount>
                    {
                        new EeeOutputAmount(ObligationType.B2B, WeeeCategory.AutomaticDispensers, tonnageForQ1, producer)
                    },
                    Quarter = new Quarter(2016, QuarterType.Q1)
                },
                new ProducerEeeByQuarter
                {
                    Eee = new List<EeeOutputAmount>
                    {
                        new EeeOutputAmount(ObligationType.B2B, WeeeCategory.AutomaticDispensers, tonnageForQ2, producer)
                    },
                    Quarter = new Quarter(2016, QuarterType.Q2)
                }
            };

            var result = map.Map(data);

            Assert.NotEmpty(result.Q1EEE);
            Assert.Equal(tonnageForQ1, result.Q1EEE.Sum(eee => eee.Tonnage));

            Assert.NotEmpty(result.Q1EEE);
            Assert.Equal(tonnageForQ2, result.Q2EEE.Sum(eee => eee.Tonnage));

            Assert.NotEmpty(result.TotalEee);
            Assert.Equal(tonnageForQ1 + tonnageForQ2, result.TotalEee.Sum(eee => eee.Tonnage));
        }
    }
}
