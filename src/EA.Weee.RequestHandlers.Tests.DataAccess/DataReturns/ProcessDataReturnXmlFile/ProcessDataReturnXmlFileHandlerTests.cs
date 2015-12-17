namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Xunit;

    public class ProcessDataReturnXmlFileHandlerTests
    {
        // TODO: Write tests

        //[Fact]
        //public void HandleAsync_SchemaErrors_NullComplianceYear()
        //{
        //    var message = new ProcessDataReturnXmlFile(Guid.NewGuid(), new byte[1], "File name");

        //    var generateFromXml = new GenerateFromXmlBuilder().Build();

        //    var result = generateFromXml.GenerateDataReturnsUpload(message,
        //        new List<DataReturnUploadError>
        //        {
        //            new DataReturnUploadError(ErrorLevel.Error, UploadErrorType.Schema, "Some schema error")
        //        }, A.Dummy<Scheme>());

        //    Assert.Null(result.ComplianceYear);
        //}

        //[Fact]
        //public void HandleAsync_NoSchemaErrors_ComplianceYearObtained()
        //{
        //    var builder = new GenerateFromXmlBuilder();
        //    A.CallTo(() => builder.XmlDeserializer.Deserialize<SchemeReturn>(A<XDocument>._))
        //        .Returns(new SchemeReturn { ComplianceYear = "2015" });

        //    var message = new ProcessDataReturnXmlFile(Guid.NewGuid(), new byte[1], "File name");
        //    var generateFromXml = builder.Build();

        //    var result = generateFromXml.GenerateDataReturnsUpload(message, new List<DataReturnUploadError>(), A.Dummy<Scheme>());

        //    Assert.NotNull(result.ComplianceYear);
        //    Assert.Equal(2015, result.ComplianceYear.Value);
        //}
    }
}
