namespace EA.Weee.Requests.Tests.Unit.MemberRegistration
{
    using System.Xml.Linq;
    using EA.Weee.RequestHandlers.PCS.MemberRegistration;
    using Xunit;

    public class XmlErrorTranslatorTests
    {
        private const int TestLineNumber = 1;

        private const string TestField = "TestField";

        private const string TestValue = "TestValue";

        private const string TestType = "TestType";

        [Fact]
        public void XmlErrorTranslator_MinInclusiveFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The MinInclusive constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = string.Format("The value '{0}' supplied for field '{1}' is too low. (Line {2}.)", TestValue, TestField, TestLineNumber);

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_MaxInclusiveFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The MaxInclusive constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = string.Format("The value '{0}' supplied for field '{1}' is too high. (Line {2}.)", TestValue, TestField, TestLineNumber);

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_PatternFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The Pattern constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = string.Format("The value '{0}' supplied for field '{1}' doesn't match the required format. (Line {2}.)", TestValue, TestField, TestLineNumber);

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_EnumerationFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The Enumeration constraint failed.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = string.Format("The value '{0}' supplied for field '{1}' isn't one of the accepted values. (Line {2}.)", TestValue, TestField, TestLineNumber);

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_MinLengthFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The actual length is less than the MinLength value.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = string.Format("The value '{0}' supplied for field '{1}' is too short. (Line {2}.)", TestValue, TestField, TestLineNumber);

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
        }

        [Fact]
        public void XmlErrorTranslator_MaxLengthFailed_CorrectMessage()
        {
            string providedException = string.Format("The '{0}' element is invalid - The value '{1}' is invalid according to its datatype '{2}' - The actual length is greater than the MaxLength value.", TestField, TestValue, TestType);
            string expectedFriendlyMessage = string.Format("The value '{0}' supplied for field '{1}' is too long. (Line {2}.)", TestValue, TestField, TestLineNumber);

            CheckExceptionMessage(expectedFriendlyMessage, providedException);
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
            return new XElement(XName.Get(TestField, "http://www.fakenamespace.com"))
            {
                Value = TestValue
            };
        }
    }
}
