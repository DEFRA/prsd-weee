namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using Domain.AatfReturn;
    using FluentAssertions;
    using Xunit;

    public class ReturnReportOnTests
    {
        [Fact]
        public void ReturnReport_OnShouldInheritFromReturnEntity()
        {
            typeof(ReturnReportOn).BaseType.Name.Should().Be(typeof(Domain.AatfReturn.ReturnEntity).Name);
        }
    }
}
