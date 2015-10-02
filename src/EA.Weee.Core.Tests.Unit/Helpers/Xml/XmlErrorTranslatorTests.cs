namespace EA.Weee.Core.Tests.Unit.Helpers.Xml
{
    using EA.Weee.Core.Helpers.Xml;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class XmlErrorTranslatorTests
    {
        /// <summary>
        /// This test ensures that an error regarding the inability to parse an element's content due
        /// to an entity parsing error will be replaced by the specified friendly error message
        /// including the line number reference.
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_EntityNameParsingError_ReturnsFriendlyMessage()
        {
            // Arrange
            string xmlValidationErrorMessage = "An error occurred while parsing EntityName. Line 123, position 456.";

            XmlErrorTranslator translator = new XmlErrorTranslator();

            // Act
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage);

            // Assert

            string expectedResult = "Your XML file is not encoded correctly. Check for any characters " +
                "which need to be encoded. For example, replace ampersand (&) characters with &amp;. (XML line 123)";

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// This test ensures that an error regarding the inability to parse an element's content due
        /// to an unterminated character entity reference will be replaced by the specified friendly error message
        /// including the line number reference.
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_UnterminatedCharacterEntityReference_ReturnsFriendlyMessage()
        {
            // Arrange
            string xmlValidationErrorMessage = "'Foo' is an unexpected token. The expected token is ';'. Line 123, position 456.";

            XmlErrorTranslator translator = new XmlErrorTranslator();

            // Act
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage);

            // Assert

            string expectedResult = "Your XML file is not encoded correctly. Check for any characters " +
                "which need to be encoded. For example, replace ampersand (&) characters with &amp;. (XML line 123)";

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// This test ensures that an error regarding the inability to parse an element's content due
        /// to an invalid character entity reference will be replaced by the specified friendly error message
        /// including the line number reference.
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_InvalidCharacterEntityReference_ReturnsFriendlyMessage()
        {
            // Arrange
            string xmlValidationErrorMessage = "Reference to undeclared entity 'Foo'. Line 123, position 456.";

            XmlErrorTranslator translator = new XmlErrorTranslator();

            // Act
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage);

            // Assert

            string expectedResult = "Your XML file is not encoded correctly. Check for any characters " +
                "which need to be encoded. For example, replace ampersand (&) characters with &amp;. (XML line 123)";

            Assert.Equal(expectedResult, result);
        }
    }
}
