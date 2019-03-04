namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfAddressTests
    {
        [Theory]
        [InlineData("")]
        public void Aatf_GivenNameIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(value, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Guid>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenNameIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(null, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Guid>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        public void Aatf_GiveAddress1IsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), value, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Guid>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenAddress1IsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), null, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Guid>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        public void Aatf_GiveTownOrCityIsEmpty_ThrowsArgumentException(string value)
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), value, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Guid>());
            };

            constructor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aatf_GivenTownOrCityIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new AatfAddress(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), null, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Guid>());
            };

            constructor.Should().Throw<ArgumentException>();
        }
    }
}
