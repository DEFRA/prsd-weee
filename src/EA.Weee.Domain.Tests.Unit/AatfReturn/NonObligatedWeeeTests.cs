namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using Xunit;

    public class NonObligatedWeeeTests
    {
        [Fact]
        public void NonObligatedWeee_AATFReturnNotDefined_ThrowsArugmentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new NonObligatedWeee(null, 2, false, 3));
        }
    }
}
