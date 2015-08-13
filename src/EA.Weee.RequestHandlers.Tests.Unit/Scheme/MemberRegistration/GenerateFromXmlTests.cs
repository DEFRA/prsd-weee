namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using DataAccess;
    using Domain;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration.GenerateProducerObjects;
    using Requests.Scheme.MemberRegistration;
    using Xunit;

    public class GenerateFromXmlTests
    {
        private readonly Guid exampleSchemeGuid;
        private readonly ProcessXMLFile exampleMessage;
        private const int ExampleComplianceYear = 2015;

        private readonly GenerateFromXml generateFromXml;

        public GenerateFromXmlTests()
        {
            exampleSchemeGuid = Guid.NewGuid();
            exampleMessage = new ProcessXMLFile(exampleSchemeGuid, new byte[1]);

            var fakeWeeeContext = A.Fake<WeeeContext>();
            var fakeXmlConverter = A.Fake<IXmlConverter>();

            A.CallTo(() => fakeXmlConverter.Deserialize(A<XDocument>.Ignored))
                .Returns(new schemeType { complianceYear = ExampleComplianceYear.ToString() });

            generateFromXml = new GenerateFromXml(fakeXmlConverter, fakeWeeeContext);
        }

        [Fact]
        public void GenerateMemberUpload_SchemaErrors_NullComplianceYear()
        {
            var result = RunGenerateFromXml(new List<MemberUploadError>
            {
                new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, "Some schema error")
            });

            Assert.Null(result.ComplianceYear);
        }

        [Fact]
        public void GenerateMemberUpload_NoSchemaErrors_ComplianceYearObtained()
        {
            var result = RunGenerateFromXml(new List<MemberUploadError>());

            Assert.NotNull(result.ComplianceYear);
            Assert.Equal(ExampleComplianceYear, result.ComplianceYear.Value);
        }

        [Fact]
        public void GenerateMemberUpload_NullSchemaErrors_ComplianceYearObtained()
        {
            var result = RunGenerateFromXml(null);

            Assert.NotNull(result.ComplianceYear);
            Assert.Equal(ExampleComplianceYear, result.ComplianceYear.Value);
        }

        private MemberUpload RunGenerateFromXml(List<MemberUploadError> errors)
        {
            return generateFromXml.GenerateMemberUpload(exampleMessage, errors, (decimal)0.0, exampleSchemeGuid);
        }
    }
}
