namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using AatfSize = Domain.AatfReturn.AatfSize;
    using AatfStatus = Domain.AatfReturn.AatfStatus;

    public class AatfTests
    {
        [Theory]
        [InlineData("")]
        public void Aatf_GivenNameIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Aatf(value, A.Dummy<UKCompetentAuthority>(), A.Dummy<string>(), AatfStatus.Approved, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenNameIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf(null, A.Dummy<UKCompetentAuthority>(), A.Dummy<string>(), AatfStatus.Approved, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenCompetentAuthorityIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approval", AatfStatus.Approved, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        public void Aatf_GivenApprovalNumberIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", A.Dummy<UKCompetentAuthority>(), value, AatfStatus.Approved, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenAatfStatusIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approvalNumber", null, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenOrganisationIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approvalNumber", null, null, A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenValidParameters_AatfPropertiesShouldBeSet()
        {
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            var organisation = A.Fake<Organisation>();
            const string name = "name";
            const string approvalNumber = "approvalNumber";
            var aatfStatus = AatfStatus.Approved;
            var contact = A.Fake<AatfContact>();
            var complianceYear = (Int16)2019;

            var aatf = new Aatf(name, competentAuthority, approvalNumber, aatfStatus, organisation, A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, contact, A.Dummy<FacilityType>(), complianceYear);

            aatf.CompetentAuthority.Should().Be(competentAuthority);
            aatf.ApprovalNumber.Should().Be(approvalNumber);
            aatf.AatfStatus.Should().Be(aatfStatus);
            aatf.Name.Should().Be(name);
            aatf.Organisation.Should().Be(organisation);
            aatf.Contact.Should().Be(contact);
            aatf.ComplianceYear.Should().Be(complianceYear);
        }
    }
}