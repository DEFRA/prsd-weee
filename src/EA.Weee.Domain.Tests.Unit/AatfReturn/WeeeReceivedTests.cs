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
        public void WeeeReceived_SchemeNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new WeeeReceived(null, A.Fake<Aatf>(), A.Dummy<Return>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReceived_AatfNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new WeeeReceived(A.Fake<Scheme>(), null, A.Dummy<Return>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReceived_GivenValidParameters_WeeeReceivedPropertiesShouldBeSet()
        {
            var scheme = A.Fake<Scheme>();
            var aatf = A.Fake<Aatf>();
            var @return = A.Fake<Return>();

            var weeeReceived = new WeeeReceived(scheme, aatf, @return);

            weeeReceived.Scheme.Should().Be(scheme);
            weeeReceived.Aatf.Should().Be(aatf);
            weeeReceived.Return.Should().Be(@return);
        }

        [Fact]
        public void WeeeReceived_ShouldInheritFromReturnEntity()
        {
            typeof(WeeeReceived).BaseType.Name.Should().Be(typeof(Domain.AatfReturn.ReturnEntity).Name);
        }
    }
}