namespace EA.Weee.Requests.Tests.Unit.MemberRegistration.XmlValidation.SchemaValidation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core.Helpers.Xml;
    using Domain;
    using FakeItEasy;
    using PCS.MemberRegistration;
    using RequestHandlers.PCS.MemberRegistration.XmlValidation.SchemaValidation;
    using Xunit;

    public class SchemaValidatorTests
    {
        private readonly IXmlErrorTranslator xmlErrorTranslator;

        public SchemaValidatorTests()
        {
            xmlErrorTranslator = A.Fake<IXmlErrorTranslator>();
        }

        [Fact]
        public void SchemaValidation_ValidXml_NoErrors()
        {
            var validXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-valid.xml");
            var validXml = File.ReadAllText(new Uri(validXmlLocation).LocalPath);

            var errors = SchemaValidator().Validate(new ProcessXmlFile(A<Guid>._, validXml));

            Assert.Empty(errors);
        }

        [Fact]
        public void SchemaValidation_NonSchemaXml_AddsError()
        {
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-slightly-invalid.xml");
            var invalidXml = File.ReadAllText(new Uri(invalidXmlLocation).LocalPath);

            var errors = SchemaValidator().Validate(new ProcessXmlFile(A<Guid>._, invalidXml));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_CorruptXml_AddsError()
        {
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\v3-badly-damaged.xml");
            var invalidXml = File.ReadAllText(new Uri(invalidXmlLocation).LocalPath);

            var errors = SchemaValidator().Validate(new ProcessXmlFile(A<Guid>._, invalidXml));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        [Fact]
        public void SchemaValidation_NonXmlFile_AddsError()
        {
            var invalidXmlLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), @"ExampleXML\not-xml.xml");
            var invalidXml = File.ReadAllText(new Uri(invalidXmlLocation).LocalPath);

            var errors = SchemaValidator().Validate(new ProcessXmlFile(A<Guid>._, invalidXml));

            Assert.NotEmpty(errors.Where(me => me.ErrorLevel == ErrorLevel.Error));
        }

        private SchemaValidator SchemaValidator()
        {
            return new SchemaValidator(xmlErrorTranslator);
        }
    }
}