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
                "which need to be encoded. For example, replace ampersand (&) characters with &amp; (XML line 123).";

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
                "which need to be encoded. For example, replace ampersand (&) characters with &amp; (XML line 123).";

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
                "which need to be encoded. For example, replace ampersand (&) characters with &amp; (XML line 123).";

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Returns friendly error message for invalid boolean data type
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_InvalidBooleanDataType_ReturnsFriendlyMessage()
        {
            // Arrange
            string value = "123456";
            string xmlType = "Boolean";
            string xmlMessage = string.Format(
                "The 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:fieldName' element is invalid - The value '{0}' is invalid according to its datatype 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:dataType' - The string '{0}' is not a valid {1} value.", value, xmlType);
            XmlErrorTranslator translator = new XmlErrorTranslator();
            XElement sender = new XElement("fieldName");
            sender.Value = value;

            // Act
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12);

            // Assert
            string expectedResult = string.Format("The value '{0}' supplied for field 'fieldName' doesn't match the required data type. The value '{0}' must be true, false, 0 or 1 (XML line 12).", value);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Returns friendly error message for invalid Date data type
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_InvalidDateDataType_ReturnsFriendlyMessage()
        {
            // Arrange
            string value = "27/10/2015";
            string xmlType = "Date";
            string xmlMessage = string.Format(
                "The 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:fieldName' element is invalid - The value '{0}' is invalid according to its datatype 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:dataType' - The string '{0}' is not a valid {1} value.", value, xmlType);
            XmlErrorTranslator translator = new XmlErrorTranslator();
            XElement sender = new XElement("fieldName");
            sender.Value = value;

            // Act
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12);

            // Assert
            string expectedResult = string.Format("The value '{0}' supplied for field 'fieldName' doesn't match the required data type. The value '{0}' must be a date in the format YYYY-MM-DD (XML line 12).", value);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Returns friendly error message for invalid Integer data type
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_InvalidIntegerDataType_ReturnsFriendlyMessage()
        {
            // Arrange
            string value = "2106.9";
            string xmlType = "Integer";
            string xmlMessage = string.Format(
                "The 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:fieldName' element is invalid - The value '{0}' is invalid according to its datatype 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:dataType' - The string '{0}' is not a valid {1} value.", value, xmlType);
            XmlErrorTranslator translator = new XmlErrorTranslator();
            XElement sender = new XElement("fieldName");
            sender.Value = value;

            // Act
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12);

            // Assert
            string expectedResult = string.Format("The value '{0}' supplied for field 'fieldName' doesn't match the required data type. The value '{0}' must be a whole number (XML line 12).", value);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Returns friendly error message for invalid decimal data type
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_InvalidDecimalDataType_ReturnsFriendlyMessage()
        {
            // Arrange
            string value = "12";
            string xmlType = "Decimal";
            string xmlMessage = string.Format(
                "The 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:fieldName' element is invalid - The value '{0}' is invalid according to its datatype 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:dataType' - The string '{0}' is not a valid {1} value.", value, xmlType);
            XmlErrorTranslator translator = new XmlErrorTranslator();
            XElement sender = new XElement("fieldName");
            sender.Value = value;

            // Act
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12);

            // Assert
            string expectedResult = string.Format("The value '{0}' supplied for field 'fieldName' doesn't match the required data type. The value '{0}' must be a decimal (XML line 12).", value);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Returns friendly error message for invalid Single data type
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_InvalidSingleDataType_ReturnsFriendlyMessage()
        {
            // Arrange
            string value = "One thousand pounds";
            string xmlType = "Single";
            string xmlMessage = string.Format(
                "The 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:fieldName' element is invalid - The value '{0}' is invalid according to its datatype 'http://www.environment-agency.gov.uk/WEEE/XMLSchema:dataType' - The string '{0}' is not a valid {1} value.", value, xmlType);
            XmlErrorTranslator translator = new XmlErrorTranslator();
            XElement sender = new XElement("fieldName");
            sender.Value = value;

            // Act
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12);

            // Assert
            string expectedResult = string.Format("The value '{0}' supplied for field 'fieldName' doesn't match the required data type. The value '{0}' must be a number (XML line 12).", value);

            Assert.Equal(expectedResult, result);
        }
    }
}
