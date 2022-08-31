namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using Core.Helpers;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class AatfSimpleMapTests
    {
        private readonly AatfSimpleMap map;

        public AatfSimpleMapTests()
        {
            map = new AatfSimpleMap(A.Fake<AatfAddressMap>(), A.Fake<AatfContactMap>(), A.Fake<OrganisationMap>(), A.Fake<AatfStatusMap>());
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
            var complianceYear = (short)2019;
            var address = A.Fake<AatfAddress>();
            var contact = A.Fake<AatfContact>();
            var organisation = A.Fake<Organisation>();
            var type = FacilityType.Ae;

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.ApprovalNumber).Returns(approvalNumber);
            A.CallTo(() => aatf.Name).Returns(name);
            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => aatf.AatfStatus).Returns(AatfStatus.Approved);
            A.CallTo(() => address.Id).Returns(Guid.NewGuid());
            A.CallTo(() => aatf.SiteAddress).Returns(address);
            A.CallTo(() => contact.Id).Returns(Guid.NewGuid());
            A.CallTo(() => aatf.Contact).Returns(contact);
            A.CallTo(() => organisation.Id).Returns(Guid.NewGuid());
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => aatf.FacilityType).Returns(type);

            var result = map.Map(new AatfSimpleMapObject(aatf));

            result.Name.Should().Be(name);
            result.ApprovalNumber.Should().Be(approvalNumber);
            result.Id.Should().Be(id);
            result.FacilityType.ToDisplayString().Should().Be(type.DisplayName);
            result.ComplianceYear.Should().Be(complianceYear);
            result.AatfStatus.Should().Be(Core.AatfReturn.AatfStatus.Approved);
            result.SiteAddress.Id.Should().Be(address.Id);
            result.Contact.Id.Should().Be(contact.Id);
            result.Organisation.Id.Should().Be(organisation.Id);
            result.AatfStatusDisplay.Should().Be(Core.AatfReturn.AatfStatus.Approved.ToDisplayString());
        }

        [Fact]
        public void Map_GivenSourceHasNullEvidenceNotes_HasEvidenceNotePropertyShouldBeFalse()
        {
            //arrange
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.AatfStatus).Returns(AatfStatus.Approved);
            A.CallTo(() => aatf.Size).Returns(AatfSize.Large);
            A.CallTo(() => aatf.Notes).Returns(null);

            //act
            var result = map.Map(new AatfSimpleMapObject(aatf));

            //assert
            result.HasEvidenceNotes.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceHasEmptyEvidenceNotes_HasEvidenceNotePropertyShouldBeFalse()
        {
            //arrange
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.AatfStatus).Returns(AatfStatus.Approved);
            A.CallTo(() => aatf.Size).Returns(AatfSize.Large);
            A.CallTo(() => aatf.Notes).Returns(new List<Note>());

            //act
            var result = map.Map(new AatfSimpleMapObject(aatf));

            //assert
            result.HasEvidenceNotes.Should().BeFalse();
        }

        [Fact]
        public void Map_GivenSourceHasEvidenceNotes_HasEvidenceNotePropertyShouldBeFalse()
        {
            //arrange
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.AatfStatus).Returns(AatfStatus.Approved);
            A.CallTo(() => aatf.Size).Returns(AatfSize.Large);
            A.CallTo(() => aatf.Notes).Returns(new List<Note>() { new Note() });

            //act
            var result = map.Map(new AatfSimpleMapObject(aatf));

            //assert
            result.HasEvidenceNotes.Should().BeTrue();
        }
    }
}
