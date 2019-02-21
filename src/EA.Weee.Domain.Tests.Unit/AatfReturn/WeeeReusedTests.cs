namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class WeeeReusedTests
    {
        [Fact]
        public void WeeeReused_AatfNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new WeeeReused(null, Guid.NewGuid());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReceived_GivenValidParameters_WeeeReceivedPropertiesShouldBeSet()
        {
            var aatf = A.Fake<Aatf>();
            var returnId = Guid.NewGuid();

            var weeeReceived = new WeeeReused(aatf, returnId);

            weeeReceived.Aatf.Should().Be(aatf);
            weeeReceived.ReturnId.Should().Be(returnId);
        }
    }
}