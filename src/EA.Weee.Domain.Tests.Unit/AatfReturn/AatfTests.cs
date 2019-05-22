﻿namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
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
                var @return = new Aatf(value, A.Dummy<UKCompetentAuthority>(), A.Dummy<string>(), AatfStatus.Approved, A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenNameIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf(null, A.Dummy<UKCompetentAuthority>(), A.Dummy<string>(), AatfStatus.Approved, A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenCompetantAuthorityIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approval", AatfStatus.Approved, A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        public void Aatf_GivenApprovalNumberIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", A.Dummy<UKCompetentAuthority>(), value, AatfStatus.Approved, A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenAatfStatusIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approvalNumber", null, A.Fake<Operator>(), A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenOperatorIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approvalNumber", null, null, A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenValidParameters_AatfPropertiesShouldBeSet()
        {
            var competantAuthority = A.Fake<UKCompetentAuthority>();
            var @operator = A.Fake<Operator>();
            const string name = "name";
            const string approvalNumber = "approvalNumber";
            var aatfStatus = AatfStatus.Approved;
            var contact = A.Fake<AatfContact>();

            var aatf = new Aatf(name, competantAuthority, approvalNumber, aatfStatus, @operator, A.Fake<AatfAddress>(), A.Fake<AatfSize>(), DateTime.Now, A.Fake<AatfContact>(), A.Dummy<FacilityType>());

            aatf.CompetentAuthority.Should().Be(competantAuthority);
            aatf.ApprovalNumber.Should().Be(approvalNumber);
            aatf.AatfStatus.Should().Be(aatfStatus);
            aatf.Name.Should().Be(name);
            aatf.Operator.Should().Be(@operator);
            aatf.Contact.Should().Be(contact);
        }
    }
}