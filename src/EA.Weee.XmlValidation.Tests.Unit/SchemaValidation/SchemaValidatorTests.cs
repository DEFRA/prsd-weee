namespace EA.Weee.XmlValidation.Tests.Unit.SchemaValidation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using Core.Shared;
    using FakeItEasy;
    using Weee.XmlValidation.Errors;
    using Weee.XmlValidation.SchemaValidation;
    using Xml.Converter;
    using Xunit;

    public class SchemaValidatorTests
    {
        private readonly IXmlErrorTranslator xmlErrorTranslator;
        private readonly IXmlConverter xmlConverter;

        public SchemaValidatorTests()
        {
            xmlErrorTranslator = A.Fake<IXmlErrorTranslator>();
            xmlConverter = A.Fake<IXmlConverter>();
        }

        [Fact]
        public void SchemaValidation_ValidXml_NoErrors()
        {
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                @"ExampleXML\v3-valid.xml");
            var validXml = File.ReadAllText(new Uri(validXmlLocation).LocalPath);

            A.CallTo(() => xmlConverter.Convert(A<byte[]>._))
                .Returns(XDocument.Parse(validXml));

            var errors = SchemaValidator().Validate(new byte[1], @"EA.Weee.Xml.MemberRegistration.v3schema.xsd", @"http://www.environment-agency.gov.uk/WEEE/XMLSchema", A<string>._);

            Assert.Empty(errors);
        }

        [Fact]
        public void SchemaValidation_EmptyFile_AddsError_WithIncorrectlyFormattedXmlMessage()
        {
            var errors = SchemaValidator()
                .Validate(new byte[0], @"EA.Weee.Xml.MemberRegistration.v3schema.xsd",
                    @"http://www.environment-agency.gov.uk/WEEE/XMLSchema", A<string>._);

            Assert.Single(errors);
            Assert.Contains(XmlErrorTranslator.IncorrectlyFormattedXmlMessage, errors.Single().Message);
        }

        [Fact]
        public void SchemaValidation_IncorrectNamespace_AddsError()
        {
            var wrongNamespaceXmlLocation = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-wrong-namespace.xml");
            var wrongNamespaceXml = File.ReadAllText(new Uri(wrongNamespaceXmlLocation).LocalPath);

            A.CallTo(() => xmlConverter.Convert(A<byte[]>._))
                .Returns(XDocument.Parse(wrongNamespaceXml));

            var errors = SchemaValidator().Validate(new byte[0], string.Empty, string.Empty, A<string>._);

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_NonSchemaXml_AddsError()
        {
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase),
                @"ExampleXML\v3-slightly-invalid.xml");
            var invalidXml = File.ReadAllText(new Uri(invalidXmlLocation).LocalPath);

            A.CallTo(() => xmlConverter.Convert(A<byte[]>._))
                .Returns(XDocument.Parse(invalidXml));

            var errors = SchemaValidator().Validate(new byte[0], string.Empty, string.Empty, A<string>._);

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_Corrupt_AddsError()
        {
            A.CallTo(() => xmlConverter.Convert(A<byte[]>._))
                .Throws<XmlException>();

            var errors = SchemaValidator().Validate(A<byte[]>._, string.Empty, string.Empty, A<string>._);

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_EmptyXml_AddsError()
        {
            A.CallTo(() => xmlConverter.Convert(A<byte[]>._)).MustNotHaveHappened();

            var errors = SchemaValidator().Validate(new byte[0], string.Empty, string.Empty, A<string>._);

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        private SchemaValidator SchemaValidator()
        {
            return new SchemaValidator(xmlErrorTranslator, xmlConverter);
        }
    }
}