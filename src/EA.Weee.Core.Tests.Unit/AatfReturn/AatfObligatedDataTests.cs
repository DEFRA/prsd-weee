namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using EA.Weee.Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;
    public class AatfObligatedDataTests
    {
        [Fact]
        public void Constructor_GivenNullAatfData_ArgumentNullExceptionExpected()
        {
            Action act = () =>
            {
                var aatfSchemeData = new AatfObligatedData(null, A.Dummy<List<AatfSchemeData>>());
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_GivenNullSchemeData_ArgumentNullExceptionExpected()
        {
            Action act = () =>
            {
                var aatfSchemeData = new AatfObligatedData(A.Dummy<AatfData>(), null);
            };

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
