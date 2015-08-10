namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using Core.Helpers;
    using Xunit;

    public class ExtensionsTests
    {
        [Theory]
        [InlineData(0, "0.00")]
        [InlineData(3.3454, "3.35")]
        [InlineData(3.4, "3.40")]
        [InlineData(354.0, "354")]
        [InlineData(1051, "1050")]
        [InlineData(1055, "1060")]
        public void DoubleTo3SignificantFigures_ReturnsExpectedString(double input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, input.ToStringWithXSignificantFigures(3));
        }
    }
}
