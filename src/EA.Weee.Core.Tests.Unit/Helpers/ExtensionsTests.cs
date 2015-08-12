namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
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

        [Fact]
        public void DomainEnumerationToCoreEnumeration_ReturnsExpectedValues()
        {
            Assert.Equal(CoreEnumeration.Something, DomainEnumeration.Something.ToCoreEnumeration<CoreEnumeration>());
            Assert.Equal(CoreEnumeration.SomethingElse, DomainEnumeration.SomethingElse.ToCoreEnumeration<CoreEnumeration>());
        }

        [Fact]
        public void DomainEnumerationToAnInvalidType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DomainEnumeration.Something.ToCoreEnumeration<string>()); // String is not an enum
        }

        [Fact]
        public void CoreEnumerationToDomainEnumeration_ReturnsExpectedValues()
        {
            Assert.Equal(DomainEnumeration.Something, CoreEnumeration.Something.ToDomainEnumeration<DomainEnumeration>());
        }
    }
}
