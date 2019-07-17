namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Lookup;
    using Xunit;

    public class ReturnAatfTests
    {
        [Fact]
        public void ReturnAatf_GivenAatfIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new ReturnAatf(null, A.Dummy<@Return>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReturnAatf_GivenReturnIsNull_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new ReturnAatf(A.Dummy<Aatf>(), null);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }
    }
}
