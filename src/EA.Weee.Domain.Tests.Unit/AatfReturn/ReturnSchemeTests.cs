namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using EA.Weee.Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReturnSchemeTests
    {
        [Fact]
        public void ReturnScheme_GivenSchemeIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new ReturnScheme(null, A.Fake<Return>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReturnScheme_GivenReturnIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new ReturnScheme(A.Fake<Domain.Scheme.Scheme>(), null);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }
    }
}
