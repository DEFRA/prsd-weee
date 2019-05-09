namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using EA.Weee.Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfContactTests
    {
        [Theory]
        [InlineData("", "-", "-", "-", "-", "-", "-")]
        [InlineData(null, "-", "-", "-", "-", "-", "-")]

        [InlineData("-", "", "-", "-", "-", "-", "-")]
        [InlineData("-", null, "-", "-", "-", "-", "-")]

        [InlineData("-", "-", "", "-", "-", "-", "-")]
        [InlineData("-", "-", null, "-", "-", "-", "-")]

        [InlineData("-", "-", "-", "", "-", "-", "-")]
        [InlineData("-", "-", "-", null, "-", "-", "-")]

        [InlineData("-", "-", "-", "-", "", "-", "-")]
        [InlineData("-", "-", "-", "-", null, "-", "-")]

        [InlineData("-", "-", "-", "-", "-", "", "-")]
        [InlineData("-", "-", "-", "-", "-", null, "-")]

        [InlineData("-", "-", "-", "-", "-", "-", "")]
        [InlineData("-", "-", "-", "-", "-", "-", null)]
        public void AatfContact_GivenNullOrEmptyRequiredParameters_ExceptionThrown(string firstName, string lastName, string position, string address, string town, string telephone, string email)
        {
            Action constructor = () =>
            {
                var @return = new AatfContact(firstName, lastName, position, address, A.Dummy<string>(), town, A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Country>(), telephone, email);
            };

            constructor.Should().Throw<ArgumentException>();
        }
    }
}
