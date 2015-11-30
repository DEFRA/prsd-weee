namespace EA.Weee.Core.Tests.Unit.Shared
{
    using EA.Weee.Core.Shared;
    using Xunit;

    public class NoFormulaeExcelSanitizerTests
    {
        /// <summary>
        /// This test ensures that a safe input string is not identified as a threat.
        /// </summary>
        [Fact]
        public void IsThreat_WithSafeInput_ReturnsFalse()
        {
            // Arrange
            string input = "This is a safe input.";

            NoFormulaeExcelSanitizer sanitizer = new NoFormulaeExcelSanitizer();

            // Act
            bool result = sanitizer.IsThreat(input);

            // Assert
            Assert.Equal(false, result);
        }

        /// <summary>
        /// This test ensures that a string starting with an equals sign is
        /// identified as a threat.
        /// </summary>
        [Fact]
        public void IsThreat_WithUnsafeInput_ReturnsTrue()
        {
            // Arrange
            string input = "=cmd|'/C ping 127.0.0.1'!A0";

            NoFormulaeExcelSanitizer sanitizer = new NoFormulaeExcelSanitizer();

            // Act
            bool result = sanitizer.IsThreat(input);

            // Assert
            Assert.Equal(true, result);
        }

        /// <summary>
        /// This test ensures that a safe input string is modified.
        /// </summary>
        [Fact]
        public void Sanitize_WithSafeInput_ReturnsInputUnmodified()
        {
            // Arrange
            string input = "This is a safe input.";

            NoFormulaeExcelSanitizer sanitizer = new NoFormulaeExcelSanitizer();

            // Act
            string result = sanitizer.Sanitize(input);

            // Assert
            Assert.Equal("This is a safe input.", result);
        }

        /// <summary>
        /// This test ensures that a string starting with an equals sign is
        /// returned without the equals sign.
        /// </summary>
        [Fact]
        public void Sanitize_WithUnsafeInput_RemovesLeadingEqualsSign()
        {
            // Arrange
            string input = "=cmd|'/C ping 127.0.0.1'!A0";

            NoFormulaeExcelSanitizer sanitizer = new NoFormulaeExcelSanitizer();

            // Act
            string result = sanitizer.Sanitize(input);

            // Assert
            Assert.Equal("cmd|'/C ping 127.0.0.1'!A0", result);
        }
    }
}
