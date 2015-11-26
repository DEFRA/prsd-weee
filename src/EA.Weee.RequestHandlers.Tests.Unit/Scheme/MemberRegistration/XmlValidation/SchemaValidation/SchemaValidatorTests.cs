namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.XmlValidation.SchemaValidation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using Domain;
    using FakeItEasy;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration.XmlValidation.SchemaValidation;
    using Requests.Scheme.MemberRegistration;
    using Weee.XmlValidation.Errors;
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

            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.Empty(errors);
        }

        [Fact]
        public void SchemaValidation_IncorrectNamespace_AddsError()
        {
            var wrongNamespaceXmlLocation = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-wrong-namespace.xml");
            var wrongNamespaceXml = File.ReadAllText(new Uri(wrongNamespaceXmlLocation).LocalPath);

            A.CallTo(() => xmlConverter.Convert(A<byte[]>._))
                .Returns(XDocument.Parse(wrongNamespaceXml));

            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

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

            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_Corrupt_AddsError()
        {
            A.CallTo(() => xmlConverter.Convert(A<byte[]>._))
                .Throws<XmlException>();

            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._, A<string>._));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_EmptyXml_AddsError()
        {
            A.CallTo(() => xmlConverter.Convert(A<byte[]>._)).MustNotHaveHappened();

            var xmlData = new byte[0];
            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, xmlData, A<string>._));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        private SchemaValidator SchemaValidator()
        {
            return new SchemaValidator(xmlErrorTranslator, xmlConverter);
        }
    }
}