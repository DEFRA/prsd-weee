﻿namespace EA.Weee.Core.Tests.Unit.Helpers.Xml
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
        /// to an unencoded ampersand will be replaced by the specified friendly error message.
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_AmpersandWithNoCharacterEntityReference_ReturnsFriendlyMessage()
        {
            // Arrange
            string xmlValidationErrorMessage = "An error occurred while parsing EntityName.";

            XmlErrorTranslator translator = new XmlErrorTranslator();

            // Act
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage);

            // Assert

            string expectedResult = "The XML file you're trying to upload has not been encoded correctly. "
                + "Please check that any ampersand characters (&) have been replaced by &amp;.";

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// This test ensures that an error regarding the inability to parse an element's content due
        /// to an unterminated character entity reference will be replaced by the specified friendly error message.
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_UnterminatedCharacterEntityReference_ReturnsFriendlyMessage()
        {
            // Arrange
            string xmlValidationErrorMessage = "'Foo' is an unexpected token. The expected token is ';'.";

            XmlErrorTranslator translator = new XmlErrorTranslator();

            // Act
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage);

            // Assert

            string expectedResult = "The XML file you're trying to upload has not been encoded correctly. "
                + "Please check that any ampersand characters (&) have been replaced by &amp;.";

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// This test ensures that an error regarding the inability to parse an element's content due
        /// to an invalid character entity reference will be replaced by the specified friendly error message.
        /// </summary>
        [Fact]
        public void MakeFriendlyErrorMessage_InvalidCharacterEntityReference_ReturnsFriendlyMessage()
        {
            // Arrange
            string xmlValidationErrorMessage = "Reference to undeclared entity 'Foo'.";

            XmlErrorTranslator translator = new XmlErrorTranslator();

            // Act
            string result = translator.MakeFriendlyErrorMessage(xmlValidationErrorMessage);

            // Assert

            string expectedResult = "The XML file you're trying to upload has not been encoded correctly. "
                + "Please check that any ampersand characters (&) have been replaced by &amp;.";

            Assert.Equal(expectedResult, result);
        }
    }
}
