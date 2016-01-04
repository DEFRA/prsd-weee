namespace EA.Weee.XmlValidation.Tests.Unit.Errors
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Linq;
    using Core.Helpers;
    using Core.Scheme;
    using FakeItEasy;
    using XmlValidation.Errors;
    using Xunit;

    public class XmlErrorTranslatorTests
    {
        private const int TestLineNumber = 1;

        private const string TestField = "TestField";

        private const string TestValue = "TestValue";

        private const string TestType = "TestType";

        private const string TestNamespace = "http://www.fakenamespace.com";

        private const string TestRegistrationNo = "TestRegistrationNo";

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
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage, A<string>._);

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
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage, A<string>._);

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
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage, A<string>._);

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
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12, A<string>._);

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
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12, A<string>._);

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
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12, A<string>._);

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
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12, A<string>._);

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
            string result = translator.MakeFriendlyErrorMessage(sender, xmlMessage, 12, A<string>._);

            // Assert
            string expectedResult = string.Format("The value '{0}' supplied for field 'fieldName' doesn't match the required data type. The value '{0}' must be a number (XML line 12).", value);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void XmlErrorTranslator_MinInclusiveFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The MinInclusive constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The value '{0}' supplied for field '{1}' is lower than the minimum or greater than the maximum allowed value", TestValue, TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_MaxInclusiveFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The MaxInclusive constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The value '{0}' supplied for field '{1}' is lower than the minimum or greater than the maximum allowed value", TestValue, TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_PatternFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The Pattern constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The value '{0}' supplied for field '{1}' doesn't match the required format.", TestValue, TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_EnumerationFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The Enumeration constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The value '{0}' supplied for field '{1}' isn't one of the accepted values.", TestValue, TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_MinLengthFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The actual length is less than the MinLength value.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The value '{0}' supplied for field '{1}' is too short.", TestValue, TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_MaxLengthFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The actual length is greater than the MaxLength value.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The value '{0}' supplied for field '{1}' is too long.", TestValue, TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_InvalidFractionDigits_CorrectMessage()
        {
            var providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The FractionDigits constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The value '{0}' supplied for field '{1}' exceeds the maximum number of allowed decimal places.", TestValue, TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_InvalidChildElement_CorrectMessage()
        {
            string providedException = string.Format("The element 'TestParentElement' in namespace '{0}' has invalid child element '{1}' in namespace '{0}'. List of possible elements expected: '{2}' in namespace '{0}'.", TestNamespace, TestField, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The field '{0}' isn't expected here. Check that you are using v3.0.7 of the XSD schema (XML template).", TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_MissingChildElement_CorrectMessage()
        {
            string providedException = string.Format("The element '{0}' in namespace '{1}' has incomplete content. List of possible elements expected: '{2}' in namespace '{1}'.", TestField, TestNamespace, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The '{0}' element is missing a child element '{1}'.", TestField, TestType));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_ErrorInXmlDocument_CorrectMessage()
        {
            const string lineNumber = "57";

            string providedException = string.Format("There is an error in XML document ({0}, 109).", lineNumber);
            string expectedFriendlyMessage = string.Format("'{0}' This can be caused by an error on this line, or before it (XML line {1}).", providedException, lineNumber);

            var translator = new XmlErrorTranslator();

            var friendlyMessage = translator.MakeFriendlyErrorMessage(providedException, A<string>._);

            Assert.Equal(expectedFriendlyMessage, friendlyMessage);
        }

        [Fact]
        public void XmlErrorTranslator_DoesntMatchAnything_OriginalMessage()
        {
            string providedException = "Here is some random text. There's no way this'll match any regex, surely.";
            string expectedFriendlyMessage = providedException;

            var translator = new XmlErrorTranslator();

            var friendlyMessage = translator.MakeFriendlyErrorMessage(providedException, A<string>._);

            Assert.Equal(expectedFriendlyMessage, friendlyMessage);
        }

        /// <summary>
        /// This test ensures that a message about data at the root level being invalid, such as would
        /// be generated by a validating a file which was not an XML document, will be replaced by
        /// a friendly message informing the user that their file is no a correctly formatted XML
        /// document.
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_WithDataAtTheRootLevelIsInvalid_ReturnsFriendlyMessage()
        {
            // Arrange
            string schemaValidationErrorMessage = "Data at the root level is invalid. Line 1, position 1.";

            XmlErrorTranslator translator = new XmlErrorTranslator();

            // Act
            string result = translator.MakeFriendlyErrorMessage(schemaValidationErrorMessage, A<string>._);

            // Assert
            Assert.Equal("The file you're trying to upload is not a correctly formatted XML file. Upload a valid XML file.", result);
        }

        /// <summary>
        /// This test ensures that error messages resulting from the incorrect use of fixed values are translated
        /// from their formal text into friendly text that includes the local name and value of the element causing
        /// the error and the line number of that XML element.
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_ForFixedValueError_ReturnsFriendlyMessage()
        {
            // Arrange
            XElement element = XElement.Parse("<foo>bar</foo>");
            string errorMessage = "The value of the 'http://some.domain.com/schema/foo' element does not equal its fixed value.";
            int lineNmber = 123;

            XmlErrorTranslator xmlErrorTranslator = new XmlErrorTranslator();

            // Act
            string result = xmlErrorTranslator.MakeFriendlyErrorMessage(
                element,
                errorMessage,
                lineNmber,
                A.Dummy<string>());

            // Assert
            Assert.Equal(
                "The value 'bar' supplied for field 'foo' is not permitted. " +
                "Only the value specified in the schema is allowed (XML line 123).",
                result);
        }

        private string AddUniversalMessageParts(string specificMessage)
        {
            var registrationNoText = string.Format("Producer {0}: ", TestRegistrationNo);
            var lineNumberText = string.Format("(XML line {0}).", TestLineNumber);

            if (!string.IsNullOrEmpty(lineNumberText))
            {
                specificMessage = specificMessage.EndsWith(".")
                    ? specificMessage.Remove(specificMessage.Length - 1)
                    : specificMessage;
            }

            return string.Format("{0}{1} {2}", registrationNoText, specificMessage, lineNumberText);
        }

        // ReSharper disable once UnusedParameter.Local (the assertion is the whole point, R#!)
        private void CheckExceptionMessage(string expectedFriendlyMessage, string providedException)
        {
            var translator = new XmlErrorTranslator();
            var testXElement = GetTestXElement();

            string schemaVersion = MemberRegistrationSchemaVersion.Version_3_07.GetAttribute<DisplayAttribute>().Name;
            var friendlyMessage = translator.MakeFriendlyErrorMessage(testXElement, providedException, TestLineNumber, schemaVersion);

            Assert.Equal(expectedFriendlyMessage, friendlyMessage);
        }

        private XElement GetTestXElement()
        {
            var element = new XElement(XName.Get(TestField, TestNamespace))
            {
                Value = TestValue,
            };

            var parent = new XElement(XName.Get("producer", TestNamespace))
            {
                Value = TestValue,
            };

            var registrationNoElement = new XElement(XName.Get("registrationNo", TestNamespace))
            {
                Value = TestRegistrationNo,
            };

            parent.Add(element);
            parent.Add(registrationNoElement);

            return element;
        }
    }
}
