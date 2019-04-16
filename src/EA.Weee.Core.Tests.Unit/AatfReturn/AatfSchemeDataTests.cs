namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using EA.Weee.Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using Xunit;

    public class AatfSchemeDataTests
    {
        [Fact]
        public void Construct_GivenNullScheme_ArgumentNullExceptionExpected()
        {
            Action act = () =>
            {
                var aatfSchemeData = new AatfSchemeData(null, A.Dummy<ObligatedCategoryValue>(), A.Dummy<string>());
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Construct_GivenObligatedReceivedData_ArgumentNullExceptionExpected()
        {
            Action act = () =>
            {
                var aatfSchemeData = new AatfSchemeData(A.Dummy<Scheme>(), null, "ABC123");
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenSource_AatfSchemeDataPropertyShouldBeSet()
        {
            var testScheme = new Scheme(Guid.NewGuid(), "Test Scheme");
            var obligatedReceivedValues = new ObligatedCategoryValue("25.00", "50.00");

            var aatfSchemeData = new AatfSchemeData(testScheme, obligatedReceivedValues, "ABC123");

            aatfSchemeData.Scheme.Should().Be(testScheme);
            aatfSchemeData.ApprovalName.Should().Be("ABC123");
            aatfSchemeData.Received.B2C.Should().Be(obligatedReceivedValues.B2C);
            aatfSchemeData.Received.B2B.Should().Be(obligatedReceivedValues.B2B);
        }
    }
}
