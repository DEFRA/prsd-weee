namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using Domain.Obligation;
    using Domain.Producer;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Domain.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using System.Collections.Generic;
    using Xunit;

    public class EeeDataMapTests
    {
        private readonly EeeDataMap map = new EeeDataMap();

        [Fact]
        public void Map_NullSource_ReturnsEmptyList()
        {
            var result = map.Map(null);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void Map_EmptySource_ReturnsEmptyList()
        {
            var source = new EeeOutputReturnVersion();

            var result = map.Map(source);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.B2B)]
        [InlineData(ObligationType.B2C, ObligationType.B2C)]
        public void Map_ValidInput_ReturnsCorrectlyMappedList(ObligationType inputObligationType, ObligationType expectedObligationType)
        {
            var registeredProducer = A.Fake<RegisteredProducer>();

            var inputVersion = new EeeOutputReturnVersion();
            inputVersion.EeeOutputAmounts.Add(new EeeOutputAmount(inputObligationType, Domain.Lookup.WeeeCategory.AutomaticDispensers, 10.5m, registeredProducer));

            var result = map.Map(inputVersion);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Should().BeEquivalentTo(new Eee(
                tonnage: 10.5m,
                category: WeeeCategory.AutomaticDispensers,
                obligationType: (Core.Shared.ObligationType)expectedObligationType));
        }

        [Fact]
        public void Map_MultipleItems_ReturnsCorrectlyMappedList()
        {
            var registeredProducer = A.Fake<RegisteredProducer>();

            var inputVersion = new EeeOutputReturnVersion();
            inputVersion.EeeOutputAmounts.Add(new EeeOutputAmount(ObligationType.B2B, Domain.Lookup.WeeeCategory.AutomaticDispensers, 10.5m, registeredProducer));
            inputVersion.EeeOutputAmounts.Add(new EeeOutputAmount(ObligationType.B2C, Domain.Lookup.WeeeCategory.ConsumerEquipment, 20.0m, registeredProducer));

            var result = map.Map(inputVersion);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(new List<Eee>
            {
                new Eee(10.5m, WeeeCategory.AutomaticDispensers, (Core.Shared.ObligationType)ObligationType.B2B),
                new Eee(20.0m, WeeeCategory.ConsumerEquipment, (Core.Shared.ObligationType)ObligationType.B2C)
            });
        }
    }
}