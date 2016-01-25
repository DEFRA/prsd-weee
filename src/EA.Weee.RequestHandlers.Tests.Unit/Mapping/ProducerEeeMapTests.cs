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
            Assert.Single(result.Q1EEE);
            Assert.Equal(tonnage, result.Q1EEE.Single().Tonnage);
        }

        [Fact]
        public void
            FirstAndSecondQuarterHasEeeDataForTheSameCategoryAndObligationType_Q1ContainsTonnage_AndQ2ContainsTonnage_AndTotalEeeContainsSingleSumOfTonnage
            ()
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

            Assert.Single(result.Q1EEE);
            Assert.Equal(tonnageForQ1, result.Q1EEE.Single().Tonnage);

            Assert.Single(result.Q2EEE);
            Assert.Equal(tonnageForQ2, result.Q2EEE.Single().Tonnage);

            Assert.Single(result.TotalEee);
            Assert.Equal(tonnageForQ1 + tonnageForQ2, result.TotalEee.Single().Tonnage);
        }

        [Fact]
        public void FirstAndSecondQuarterHasEeeDataForTheSameCategoryButDifferentObligationType_Q1ContainsTonnage_AndQ2ContainsTonnage_AndTotalEeeTonnageIsSeparatedIntoObligationTypes()
        {
            var map = new ProducerEeeMap();

            var scheme = new Scheme(Guid.NewGuid());
            var producer = new RegisteredProducer("ABC12345", 2016, scheme);
            const decimal tonnageForQ1 = 10M;
            const decimal tonnageForQ2 = 25.6M;
            const ObligationType obligationTypeForQ1 = ObligationType.B2B;
            const ObligationType obligationTypeForQ2 = ObligationType.B2C;

            var data = new List<ProducerEeeByQuarter>
            {
                new ProducerEeeByQuarter
                {
                    Eee = new List<EeeOutputAmount>
                    {
                        new EeeOutputAmount(obligationTypeForQ1, WeeeCategory.AutomaticDispensers, tonnageForQ1, producer)
                    },
                    Quarter = new Quarter(2016, QuarterType.Q1)
                },
                new ProducerEeeByQuarter
                {
                    Eee = new List<EeeOutputAmount>
                    {
                        new EeeOutputAmount(obligationTypeForQ2, WeeeCategory.AutomaticDispensers, tonnageForQ2, producer)
                    },
                    Quarter = new Quarter(2016, QuarterType.Q2)
                }
            };

            var result = map.Map(data);

            Assert.Single(result.Q1EEE);
            Assert.Equal(tonnageForQ1, result.Q1EEE.Single().Tonnage);

            Assert.Single(result.Q2EEE);
            Assert.Equal(tonnageForQ2, result.Q2EEE.Single().Tonnage);

            Assert.Equal(2, result.TotalEee.Count);
            Assert.Single(result.TotalEee.Where(eee => eee.Tonnage == tonnageForQ1 && (ObligationType)(int)eee.ObligationType == obligationTypeForQ1));
            Assert.Single(result.TotalEee.Where(eee => eee.Tonnage == tonnageForQ2 && (ObligationType)(int)eee.ObligationType == obligationTypeForQ2));
        }

        [Fact]
        public void FirstAndSecondQuarterHasEeeDataForTheSameObligationTypeButDifferentCategory_Q1ContainsTonnage_AndQ2ContainsTonnage_AndTotalEeeTonnageIsSeparatedIntoTwoCategoryTypes()
        {
            var map = new ProducerEeeMap();

            var scheme = new Scheme(Guid.NewGuid());
            var producer = new RegisteredProducer("ABC12345", 2016, scheme);
            const decimal tonnageForQ1 = 10M;
            const decimal tonnageForQ2 = 25.6M;
            const WeeeCategory categoryForQ1 = WeeeCategory.AutomaticDispensers;
            const WeeeCategory categoryForQ2 = WeeeCategory.ConsumerEquipment;

            var data = new List<ProducerEeeByQuarter>
            {
                new ProducerEeeByQuarter
                {
                    Eee = new List<EeeOutputAmount>
                    {
                        new EeeOutputAmount(ObligationType.B2B, categoryForQ1, tonnageForQ1, producer)
                    },
                    Quarter = new Quarter(2016, QuarterType.Q1)
                },
                new ProducerEeeByQuarter
                {
                    Eee = new List<EeeOutputAmount>
                    {
                        new EeeOutputAmount(ObligationType.B2B, categoryForQ2, tonnageForQ2, producer)
                    },
                    Quarter = new Quarter(2016, QuarterType.Q2)
                }
            };

            var result = map.Map(data);

            Assert.Single(result.Q1EEE);
            Assert.Equal(tonnageForQ1, result.Q1EEE.Single().Tonnage);

            Assert.Single(result.Q2EEE);
            Assert.Equal(tonnageForQ2, result.Q2EEE.Single().Tonnage);

            Assert.Equal(2, result.TotalEee.Count);
            Assert.Single(result.TotalEee.Where(eee => eee.Tonnage == tonnageForQ1 && (WeeeCategory)(int)eee.Category == categoryForQ1));
            Assert.Single(result.TotalEee.Where(eee => eee.Tonnage == tonnageForQ2 && (WeeeCategory)(int)eee.Category == categoryForQ2));
        }
    }
}
