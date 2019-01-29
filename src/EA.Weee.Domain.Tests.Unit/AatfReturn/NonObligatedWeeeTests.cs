namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using Domain.AatfReturn;
    using FakeItEasy;
    using Xunit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NonObligatedWeeeTests
    {
        [Fact]
        public void NonObligatedWeee_AATFReturnNotDefined_ThrowsArugmentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new NonObligatedWeee(null, 2, false, 3));
        }
    }
}
