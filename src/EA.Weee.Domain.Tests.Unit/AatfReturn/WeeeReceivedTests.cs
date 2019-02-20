namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    public class WeeeReceivedTests
    {
        [Fact]
        public void WeeeReceived_SchemeNotDefined_ThrowsArugmentNullException()
        {
            Action constructor = () =>
            {
                var @return = new WeeeReceived(null, A.Fake<Aatf>(), Guid.NewGuid());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReceived_AatfNotDefined_ThrowsArugmentNullException()
        {
            Action constructor = () =>
            {
                var @return = new WeeeReceived(A.Fake<Scheme>(), null, Guid.NewGuid());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReceived_GivenValidParameters_WeeeReceivedPropertiesShouldBeSet()
        {
            var scheme = A.Fake<Scheme>();
            var aatf = A.Fake<Aatf>();
            var returnId = Guid.NewGuid();

            var weeeReceived = new WeeeReceived(scheme, aatf, returnId);

            weeeReceived.Scheme.Should().Be(scheme);
            weeeReceived.Aatf.Should().Be(aatf);
            weeeReceived.ReturnId.Should().Be(returnId);
        }
    }
}