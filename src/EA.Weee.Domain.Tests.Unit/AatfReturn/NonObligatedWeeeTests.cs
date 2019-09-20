namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using Domain.AatfReturn;
    using FluentAssertions;
    using System;
    using Xunit;

    public class NonObligatedWeeeTests
    {
        [Fact]
        public void NonObligatedWeee_AATFReturnNotDefined_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new NonObligatedWeee(null, 2, false, 3));
        }

        [Fact]
        public void NonObligatedWeee_ShouldInheritFromReturnEntity()
        {
            typeof(NonObligatedWeee).BaseType.Name.Should().Be(typeof(Domain.AatfReturn.ReturnEntity).Name);
        }
    }
}
