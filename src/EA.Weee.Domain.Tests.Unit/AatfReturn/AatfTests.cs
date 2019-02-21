namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfTests
    {
        [Theory]
        [InlineData("")]
        public void Aatf_GivenNameIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Aatf(value, A.Dummy<UKCompetentAuthority>(), A.Dummy<string>(), AatfStatus.Approved, A.Fake<Operator>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenNameIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf(null, A.Dummy<UKCompetentAuthority>(), A.Dummy<string>(), AatfStatus.Approved, A.Fake<Operator>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenCompetantAuthorityIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approval", AatfStatus.Approved, A.Fake<Operator>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        public void Aatf_GivenApprovalNumberIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", A.Dummy<UKCompetentAuthority>(), value, AatfStatus.Approved, A.Fake<Operator>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenAatfStatusIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approvalNumber", null, A.Fake<Operator>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Aatf_GivenOperatorIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new Aatf("name", null, "approvalNumber", null, null);
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

            var aatf = new Aatf(name, competantAuthority, approvalNumber, aatfStatus, @operator);

            aatf.CompetentAuthority.Should().Be(competantAuthority);
            aatf.ApprovalNumber.Should().Be(approvalNumber);
            aatf.AatfStatus.Should().Be(aatfStatus);
            aatf.Name.Should().Be(name);
            aatf.Operator.Should().Be(@operator);
        }
    }
}