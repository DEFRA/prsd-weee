﻿namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Lookup;
    using System;
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
                var @return = new Aatf(value, A.Dummy<UKCompetentAuthority>(), A.Dummy<string>(), AatfStatus.Approved, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>(), A.Dummy<LocalArea>(), A.Dummy<PanArea>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenNameIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf(null, A.Dummy<UKCompetentAuthority>(), A.Dummy<string>(), AatfStatus.Approved, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>(), A.Dummy<LocalArea>(), A.Dummy<PanArea>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenCompetentAuthorityIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approval", AatfStatus.Approved, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>(), A.Dummy<LocalArea>(), A.Dummy<PanArea>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        public void Aatf_GivenApprovalNumberIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", A.Dummy<UKCompetentAuthority>(), value, AatfStatus.Approved, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>(), A.Dummy<LocalArea>(), A.Dummy<PanArea>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenAatfStatusIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approvalNumber", null, A.Fake<Organisation>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>(), A.Dummy<LocalArea>(), A.Dummy<PanArea>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenOrganisationIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approvalNumber", null, null, A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>(), A.Dummy<Int16>(), A.Dummy<LocalArea>(), A.Dummy<PanArea>());
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
            var localArea = A.Fake<LocalArea>();
            var panArea = A.Fake<PanArea>();
            var facilityType = FacilityType.Aatf;
            var date = DateTime.Now;
            var address = A.Fake<AatfAddress>();
            var size = AatfSize.Large;

            var aatf = new Aatf(name, competentAuthority, approvalNumber, aatfStatus, organisation, address, size, date, contact, facilityType, complianceYear, localArea, panArea);

            aatf.CompetentAuthority.Should().Be(competentAuthority);
            aatf.ApprovalNumber.Should().Be(approvalNumber);
            aatf.AatfStatus.Should().Be(aatfStatus);
            aatf.Name.Should().Be(name);
            aatf.Organisation.Should().Be(organisation);
            aatf.Contact.Should().Be(contact);
            aatf.ComplianceYear.Should().Be(complianceYear);
            aatf.ApprovalDate.Should().Be(date);
            aatf.Size.Should().Be(size);
            aatf.FacilityType.Should().Be(facilityType);
            aatf.SiteAddress.Should().Be(address);
            aatf.LocalArea.Should().Be(localArea);
            aatf.PanArea.Should().Be(panArea);
            aatf.AatfId.Should().NotBeEmpty();
        }

        [Fact]
        public void Aatf_GivenNullLocalArea_LocalAreaIsNull()
        {
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            var organisation = A.Fake<Organisation>();
            const string name = "name";
            const string approvalNumber = "approvalNumber";
            var aatfStatus = AatfStatus.Approved;
            var contact = A.Fake<AatfContact>();
            var complianceYear = (Int16)2019;
            LocalArea localArea = null;
            var panArea = A.Fake<PanArea>();
            var facilityType = FacilityType.Aatf;
            var date = DateTime.Now;
            var address = A.Fake<AatfAddress>();
            var size = AatfSize.Large;

            var aatf = new Aatf(name, competentAuthority, approvalNumber, aatfStatus, organisation, address, size, date, contact, facilityType, complianceYear, localArea, panArea);

            aatf.LocalArea.Should().Be(localArea);
        }

        [Fact]
        public void Aatf_GivenNullPanArea_PanAreaIsNull()
        {
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            var organisation = A.Fake<Organisation>();
            const string name = "name";
            const string approvalNumber = "approvalNumber";
            var aatfStatus = AatfStatus.Approved;
            var contact = A.Fake<AatfContact>();
            var complianceYear = (Int16)2019;
            var localArea = A.Fake<LocalArea>();
            PanArea panArea = null;
            var facilityType = FacilityType.Aatf;
            var date = DateTime.Now;
            var address = A.Fake<AatfAddress>();
            var size = AatfSize.Large;

            var aatf = new Aatf(name, competentAuthority, approvalNumber, aatfStatus, organisation, address, size, date, contact, facilityType, complianceYear, localArea, panArea);

            aatf.PanArea.Should().Be(panArea);
        }

        [Fact]
        public void Aatf_GivenValidParametersWithAatfId_AatfPropertiesShouldBeSet()
        {
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            var organisation = A.Fake<Organisation>();
            const string name = "name";
            const string approvalNumber = "approvalNumber";
            var aatfStatus = AatfStatus.Approved;
            var contact = A.Fake<AatfContact>();
            var complianceYear = (Int16)2019;
            var localArea = A.Fake<LocalArea>();
            var panArea = A.Fake<PanArea>();
            var facilityType = FacilityType.Aatf;
            var date = DateTime.Now;
            var address = A.Fake<AatfAddress>();
            var size = AatfSize.Large;
            var aatfId = Guid.NewGuid();

            var aatf = new Aatf(name, competentAuthority, approvalNumber, aatfStatus, organisation, address, size, date, contact, facilityType, complianceYear, localArea, panArea, aatfId);

            aatf.CompetentAuthority.Should().Be(competentAuthority);
            aatf.ApprovalNumber.Should().Be(approvalNumber);
            aatf.AatfStatus.Should().Be(aatfStatus);
            aatf.Name.Should().Be(name);
            aatf.Organisation.Should().Be(organisation);
            aatf.Contact.Should().Be(contact);
            aatf.ComplianceYear.Should().Be(complianceYear);
            aatf.ApprovalDate.Should().Be(date);
            aatf.Size.Should().Be(size);
            aatf.FacilityType.Should().Be(facilityType);
            aatf.SiteAddress.Should().Be(address);
            aatf.LocalArea.Should().Be(localArea);
            aatf.PanArea.Should().Be(panArea);
            aatf.AatfId.Should().Be(aatfId);
        }
    }
}