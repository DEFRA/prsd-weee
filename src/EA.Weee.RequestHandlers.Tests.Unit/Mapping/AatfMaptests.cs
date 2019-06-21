namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using System;
    using Xunit;

    public class AatfMapTests
    {
        private readonly Fixture fixture;
        private readonly AatfMap map;
        private readonly IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType> typeMapper;

        public AatfMapTests()
        {
            typeMapper = A.Fake<IMap<Domain.AatfReturn.FacilityType, Core.AatfReturn.FacilityType>>();
            map = new AatfMap(A.Fake<UKCompetentAuthorityMap>(), A.Fake<AatfStatusMap>(), A.Fake<AatfSizeMap>(), A.Fake<AatfAddressMap>(), A.Fake<AatfContactMap>(), A.Fake<OrganisationMap>(), typeMapper, A.Fake<PanAreaMap>(), A.Fake<LocalAreaMap>());
            fixture = new Fixture();
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
            var complianceYear = (Int16)2019;
            var competentAuthority = new UKCompetentAuthority(Guid.NewGuid(), A.Dummy<string>(), A.Dummy<string>(), A.Fake<Country>(), A.Dummy<string>(), null);
            var address = A.Fake<AatfAddress>();
            var contact = A.Fake<AatfContact>();
            var organisation = A.Fake<Organisation>();
            FacilityType type = FacilityType.Ae;

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.ApprovalNumber).Returns(approvalNumber);
            A.CallTo(() => aatf.Name).Returns(name);
            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => aatf.CompetentAuthority).Returns(competentAuthority);
            A.CallTo(() => aatf.AatfStatus).Returns(AatfStatus.Approved);
            A.CallTo(() => aatf.Size).Returns(AatfSize.Large);
            A.CallTo(() => address.Id).Returns(Guid.NewGuid());
            A.CallTo(() => aatf.SiteAddress).Returns(address);
            A.CallTo(() => contact.Id).Returns(Guid.NewGuid());
            A.CallTo(() => aatf.Contact).Returns(contact);
            A.CallTo(() => organisation.Id).Returns(Guid.NewGuid());
            A.CallTo(() => aatf.Organisation).Returns(organisation);

            A.CallTo(() => aatf.FacilityType).Returns(type);

            A.CallTo(() => typeMapper.Map(type)).Returns(Core.AatfReturn.FacilityType.Ae);

            var result = map.Map(aatf);

            result.Name.Should().Be(name);
            result.ApprovalNumber.Should().Be(approvalNumber);
            result.Id.Should().Be(id);
            result.FacilityType.ToDisplayString().Should().Be(type.DisplayName);

            A.CallTo(() => typeMapper.Map(type)).MustHaveHappened(Repeated.Exactly.Once);
            result.ComplianceYear.Should().Be(complianceYear);
            result.AatfStatus.Should().Be(Core.AatfReturn.AatfStatus.Approved);
            result.Size.Should().Be(Core.AatfReturn.AatfSize.Large);
            result.CompetentAuthority.Id.Should().Be(competentAuthority.Id);
            result.SiteAddress.Id.Should().Be(address.Id);
            result.Contact.Id.Should().Be(contact.Id);
            result.Organisation.Id.Should().Be(organisation.Id);
        }
    }
}
