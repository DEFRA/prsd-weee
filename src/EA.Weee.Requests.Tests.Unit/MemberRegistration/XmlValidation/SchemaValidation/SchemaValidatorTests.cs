namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation.SchemaValidation
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using Core.Helpers.Xml;
    using Domain;
    using FakeItEasy;
    using PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.SchemaValidation;
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
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-valid.xml");
            var validXml = File.ReadAllText(new Uri(validXmlLocation).LocalPath);

            A.CallTo(() => xmlConverter.Convert(A<ProcessXMLFile>._))
                .Returns(XDocument.Parse(validXml));

            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._));

            Assert.Empty(errors);
        }

        [Fact]
        public void SchemaValidation_NonSchemaXml_AddsError()
        {
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-slightly-invalid.xml");
            var invalidXml = File.ReadAllText(new Uri(invalidXmlLocation).LocalPath);

            A.CallTo(() => xmlConverter.Convert(A<ProcessXMLFile>._))
                .Returns(XDocument.Parse(invalidXml));

            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_Corrupt_AddsError()
        {
            A.CallTo(() => xmlConverter.Convert(A<ProcessXMLFile>._))
                .Throws<XmlException>();

            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, A<byte[]>._));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_EmptyXml_AddsError()
        {
            A.CallTo(() => xmlConverter.Convert(A<ProcessXMLFile>._)).MustNotHaveHappened();

            byte[] xmlData = new byte[0];
            var errors = SchemaValidator().Validate(new ProcessXMLFile(A<Guid>._, xmlData));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        private SchemaValidator SchemaValidator()
        {
            return new SchemaValidator(xmlErrorTranslator, xmlConverter);
        }
    }
}