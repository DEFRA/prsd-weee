namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class AatfMapTests
    {
        private readonly AatfMap map;
        private readonly IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> typeMapper;

        public AatfMapTests()
        {
            typeMapper = A.Fake<IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType>>();
            map = new AatfMap(A.Fake<UKCompetentAuthorityMap>(), A.Fake<AatfStatusMap>(), A.Fake<AatfSizeMap>(), A.Fake<AatfAddressMap>(), A.Fake<AatfContactMap>(), A.Fake<OrganisationMap>(), typeMapper);
        }

        [Fact]
        public void Map_GivenSourceIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_AatfDataPropertiesShouldBeMapped()
        {
            const string name = "name";
            const string approvalNumber = "approval";
            var id = Guid.NewGuid();
            FacilityType type = FacilityType.Ae;

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.ApprovalNumber).Returns(approvalNumber);
            A.CallTo(() => aatf.Name).Returns(name);
            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.FacilityType).Returns(type);

            A.CallTo(() => typeMapper.Map(type)).Returns(Core.AatfReturn.FacilityType.Ae);

            var result = map.Map(aatf);

            result.Name.Should().Be(name);
            result.ApprovalNumber.Should().Be(approvalNumber);
            result.Id.Should().Be(id);
            result.FacilityType.ToDisplayString().Should().Be(type.DisplayName);

            A.CallTo(() => typeMapper.Map(type)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
