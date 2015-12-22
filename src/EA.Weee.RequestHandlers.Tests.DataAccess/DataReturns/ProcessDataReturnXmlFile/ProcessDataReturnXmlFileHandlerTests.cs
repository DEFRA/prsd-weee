namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Security;
    using Weee.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xml.DataReturns;
    using XmlValidation.Errors;
    using Xunit;
    using ProcessDataReturnXmlFile = Requests.DataReturns.ProcessDataReturnXmlFile;
    using Scheme = Domain.Scheme.Scheme;

    public class ProcessDataReturnXmlFileHandlerTests
    {
        [Fact]
        public async Task HandleAsync_XmlContainsSchemaError_CreatesDataReturnUpload_WithNullComplianceYearAndQuarter()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                helper.CreateScheme(organisation);

                database.Model.SaveChanges();

                var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(
                    "Test XML string",
                    A.Dummy<SchemeReturn>(),
                    new List<XmlValidationError> { new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, "Error text") });

                var builder = new ProcessDataReturnXmlFileHandlerBuilder(database.WeeeContext);

                A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                     .Returns(xmlGeneratorResult);

                // Act
                var dataReturnUploadId = await builder.InvokeHandleAsync(organisation.Id);

                //Assert
                var dataReturnUpload = domainHelper.GetDataReturnUpload(dataReturnUploadId);

                Assert.Null(dataReturnUpload.ComplianceYear);
                Assert.Null(dataReturnUpload.Quarter);
            }
        }

        private class ProcessDataReturnXmlFileHandlerBuilder
        {
            private readonly IProcessDataReturnXmlFileDataAccess dataAccess;
            private readonly IWeeeAuthorization authorization;
            public IGenerateFromDataReturnXml XmlGenerator;
            public IDataReturnVersionFromXmlBuilder DataReturnVersionFromXmlBuilder;
            private readonly Func<IDataReturnVersionBuilder, IDataReturnVersionFromXmlBuilder> dataReturnVersionFromXmlBuilderDelegate;
            private readonly Func<Scheme, Quarter, IDataReturnVersionBuilder> dataReturnVersionBuilderDelegate;

            public ProcessDataReturnXmlFileHandlerBuilder(WeeeContext context)
            {
                dataAccess = new ProcessDataReturnXmlFileDataAccess(context);
                authorization = A.Fake<IWeeeAuthorization>();
                XmlGenerator = A.Fake<IGenerateFromDataReturnXml>();

                DataReturnVersionFromXmlBuilder = A.Fake<IDataReturnVersionFromXmlBuilder>();
                dataReturnVersionFromXmlBuilderDelegate = x => DataReturnVersionFromXmlBuilder;

                dataReturnVersionBuilderDelegate = A.Fake<Func<Scheme, Quarter, IDataReturnVersionBuilder>>();
            }

            public ProcessDataReturnXmlFileHandler Build()
            {
                return new ProcessDataReturnXmlFileHandler(
                                              dataAccess,
                                              authorization,
                                              XmlGenerator,
                                              dataReturnVersionFromXmlBuilderDelegate,
                                              dataReturnVersionBuilderDelegate);
            }

            public Task<Guid> InvokeHandleAsync(Guid organisationId, byte[] data = null, string fileName = null)
            {
                var messageData = data ?? A.Dummy<byte[]>();
                var messageFileName = fileName ?? A.Dummy<string>();

                return Build().HandleAsync(new ProcessDataReturnXmlFile(organisationId, messageData, messageFileName));
            }
        }
    }
}
