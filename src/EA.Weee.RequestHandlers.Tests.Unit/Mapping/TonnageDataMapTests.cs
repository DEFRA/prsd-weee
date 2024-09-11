namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Obligation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using System.Linq;
    using Xunit;

    public class TonnageDataMapTests
    {
        private readonly TonnageDataMap map = new TonnageDataMap();

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

        [Fact]
        public void Map_SingleCategory_MapsCorrectly()
        {
            var source = new EeeOutputReturnVersion();
            source.AddEeeOutputAmount(new EeeOutputAmount(ObligationType.B2C, WeeeCategory.AutomaticDispensers, 10, A.Fake<RegisteredProducer>()));
            source.AddEeeOutputAmount(new EeeOutputAmount(ObligationType.B2B, WeeeCategory.AutomaticDispensers, 20, A.Fake<RegisteredProducer>()));
   
            var result = map.Map(source);

            result.Should().HaveCount(1);
            result[0].CategoryId.Should().Be(WeeeCategory.AutomaticDispensers);
            result[0].HouseHold.Should().Be(10);
            result[0].NonHouseHold.Should().Be(20);
        }

        [Fact]
        public void Map_MultipleCategories_MapsCorrectly()
        {
            var source = new EeeOutputReturnVersion();

            source.AddEeeOutputAmount(new EeeOutputAmount(ObligationType.B2C, WeeeCategory.AutomaticDispensers, 10, A.Fake<RegisteredProducer>()));
            source.AddEeeOutputAmount(new EeeOutputAmount(ObligationType.B2B, WeeeCategory.AutomaticDispensers, 20, A.Fake<RegisteredProducer>()));
            source.AddEeeOutputAmount(new EeeOutputAmount(ObligationType.B2C, WeeeCategory.ConsumerEquipment, 30, A.Fake<RegisteredProducer>()));
            source.AddEeeOutputAmount(new EeeOutputAmount(ObligationType.B2B, WeeeCategory.ConsumerEquipment, 40, A.Fake<RegisteredProducer>()));

            var result = map.Map(source);

            result.Should().HaveCount(2);

            var automaticDispensers = result.FirstOrDefault(r => r.CategoryId == WeeeCategory.AutomaticDispensers);
            automaticDispensers.Should().NotBeNull();
            automaticDispensers.HouseHold.Should().Be(10);
            automaticDispensers.NonHouseHold.Should().Be(20);

            var consumerEquipment = result.FirstOrDefault(r => r.CategoryId == WeeeCategory.ConsumerEquipment);
            consumerEquipment.Should().NotBeNull();
            consumerEquipment.HouseHold.Should().Be(30);
            consumerEquipment.NonHouseHold.Should().Be(40);
        }

        [Fact]
        public void Map_MissingObligationType_ReturnsNullForMissingType()
        {
            var source = new EeeOutputReturnVersion();

            source.AddEeeOutputAmount(new EeeOutputAmount(ObligationType.B2C, WeeeCategory.AutomaticDispensers, 10, A.Fake<RegisteredProducer>()));
            source.AddEeeOutputAmount(new EeeOutputAmount(ObligationType.B2B, WeeeCategory.ConsumerEquipment, 20, A.Fake<RegisteredProducer>()));

            var result = map.Map(source);

            result.Should().HaveCount(2);

            var automaticDispensers = result.FirstOrDefault(r => r.CategoryId == WeeeCategory.AutomaticDispensers);
            automaticDispensers.Should().NotBeNull();
            automaticDispensers.HouseHold.Should().Be(10);
            automaticDispensers.NonHouseHold.Should().BeNull();

            var consumerEquipment = result.FirstOrDefault(r => r.CategoryId == WeeeCategory.ConsumerEquipment);
            consumerEquipment.Should().NotBeNull();
            consumerEquipment.HouseHold.Should().BeNull();
            consumerEquipment.NonHouseHold.Should().Be(20);
        }
    }
}