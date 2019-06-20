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
                var @return = new WeeeReused(null, A.Dummy<Return>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReused_GivenValidParameters_WeeeReceivedPropertiesShouldBeSet()
        {
            var aatf = A.Fake<Aatf>();
            var @return = A.Fake<Return>();

            var weeeReceived = new WeeeReused(aatf, @return);

            weeeReceived.Aatf.Should().Be(aatf);
            weeeReceived.Return.Should().Be(@return);
        }

        [Fact]
        public void WeeeReused_ShouldInheritFromReturnEntity()
        {
            typeof(WeeeReused).BaseType.Name.Should().Be(typeof(Domain.AatfReturn.ReturnEntity).Name);
        }
    }
}