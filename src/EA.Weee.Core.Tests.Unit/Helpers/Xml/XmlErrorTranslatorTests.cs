namespace EA.Weee.Core.Tests.Unit.Helpers.Xml
{
    using System.Xml.Linq;
    using Core.Helpers.Xml;
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

        /// <summary>
        /// Returns friendly error message for invalid data types
        /// </summary>
        [Theory]
        [InlineData("123456", "Boolean")]
        [InlineData("2106.9", "Integer")]
        [InlineData("27/10/2015", "Date")]
        [InlineData("12", "Decimal")]
        [InlineData("One thousand pounds", "Single")]
        public void MakeFriendlyErrorMessage_InvalidDataType_ReturnsFriendlyMessage(string value, string xmlType)
        {
            // Arrange
            string xmlMessage = string.Format(
                "The 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:fieldName' element is invalid - The value '{0}' is invalid according to its datatype 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:dataType' - The string '{0}' is not a valid {1} value.", value, xmlType);
            XmlErrorTranslator translator = new XmlErrorTranslator();
            XElement sender = new XElement("fieldName");
            sender.Value = value;

            // Act
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12);

            // Assert
            string dataTypeMessage = GetFriendlyDataType(xmlType);
            string expectedResult = string.Format("The value '{0}' supplied for field 'fieldName' doesn't match the required data type. The value '{0}' must be a {1}. (XML line 12)", value, dataTypeMessage);

            Assert.Equal(expectedResult, result);
        }

        private string GetFriendlyDataType(string xmlType)
        {
            string friendlyDataTypeMessage = string.Empty;
            switch (xmlType)
            {
                case "Integer":
                    friendlyDataTypeMessage = "whole number";
                    break;

                case "Boolean":
                    friendlyDataTypeMessage = "true, false, 0 or 1";
                    break;

                case "Decimal":
                case "Float":
                    friendlyDataTypeMessage = "decimal";
                    break;

                case "Date":
                    friendlyDataTypeMessage = "date in the format YYYY-MM-DD";
                    break;

                case "Single":
                    friendlyDataTypeMessage = "number";
                    break;
            }
            return friendlyDataTypeMessage;
        }
    }
}
