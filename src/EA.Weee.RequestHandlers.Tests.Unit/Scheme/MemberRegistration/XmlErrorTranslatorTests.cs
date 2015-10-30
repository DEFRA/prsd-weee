namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System.Xml.Linq;
    using Core.Helpers.Xml;
    using Xunit;

    public class XmlErrorTranslatorTests
    {
        private const int TestLineNumber = 1;

        private const string TestField = "TestField";

        private const string TestValue = "TestValue";

        private const string TestType = "TestType";

        private const string TestNamespace = "http://www.fakenamespace.com";

        private const string TestRegistrationNo = "TestRegistrationNo";

        [Fact]
        public void XmlErrorTranslator_MinInclusiveFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The MinInclusive constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The {1} you've provided is incorrect. Please make sure you enter the right value.", TestValue, TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_MaxInclusiveFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The MaxInclusive constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The {1} you've provided is incorrect. Please make sure you enter the right value.", TestValue, TestField));

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
        public void XmlErrorTranslator_InvalidChildElement_CorrectMessage()
        {
            string providedException = string.Format("The element 'TestParentElement' in namespace '{0}' has invalid child element '{1}' in namespace '{0}'. List of possible elements expected: '{2}' in namespace '{0}'.", TestNamespace, TestField, TestType);
            string expectedFriendlyMessage = AddUniversalMessageParts(string.Format("The field {0} isn't expected here. Please check that you are using v3.0.7 of the XSD schema (XML template).", TestField));

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_ErrorInXmlDocument_CorrectMessage()
        {
            const string lineNumber = "57";

            string providedException = string.Format("There is an error in XML document ({0}, 109).", lineNumber);
            string expectedFriendlyMessage = string.Format("{0} This can be caused by an error on this line, or before it (XML line {1}).", providedException, lineNumber);

            var translator = new XmlErrorTranslator();

            var friendlyMessage = translator.MakeFriendlyErrorMessage(providedException);

            Assert.Equal(expectedFriendlyMessage, friendlyMessage);
        }

        [Fact]
        public void XmlErrorTranslator_DoesntMatchAnything_OriginalMessage()
        {
            string providedException = "Here is some random text. There's no way this'll match any regex, surely.";
            string expectedFriendlyMessage = providedException;

            var translator = new XmlErrorTranslator();

            var friendlyMessage = translator.MakeFriendlyErrorMessage(providedException);

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
            string result = translator.MakeFriendlyErrorMessage(schemaValidationErrorMessage);

            // Assert
            Assert.Equal("The file you're trying to upload is not a correctly formatted XML file. Please make sure you're uploading a valid XML file.", result);
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

            var friendlyMessage = translator.MakeFriendlyErrorMessage(testXElement, providedException, TestLineNumber);

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
