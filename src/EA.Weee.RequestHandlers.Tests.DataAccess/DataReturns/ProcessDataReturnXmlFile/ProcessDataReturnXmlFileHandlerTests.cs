namespace EA.Weee.RequestHandlers.Tests.DataAccess.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
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

                var builder = new ProcessDataReturnXmlFileHandlerBuilder(database.WeeeContext);

                var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(
                    "Test XML string",
                    A.Dummy<SchemeReturn>(),
                    new List<XmlValidationError> { new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, "Error text") });

                A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                     .Returns(xmlGeneratorResult);

                // Act
                var dataReturnUploadId = await builder.InvokeHandleAsync(organisation.Id);

                // Assert
                var dataReturnUpload = domainHelper.GetDataReturnUpload(dataReturnUploadId);

                Assert.Null(dataReturnUpload.ComplianceYear);
                Assert.Null(dataReturnUpload.Quarter);
            }
        }

        [Fact]
        public async Task HandleAsync_XmlContainsSchemaError_StoresAvailableDataReturnData()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                database.Model.SaveChanges();

                var builder = new ProcessDataReturnXmlFileHandlerBuilder(database.WeeeContext);

                var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(
                    "Test XML string",
                    A.Dummy<SchemeReturn>(),
                    new List<XmlValidationError> { new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, "Error text") });

                A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                     .Returns(xmlGeneratorResult);

                // Act
                var dataReturnUploadId = await builder.InvokeHandleAsync(organisation.Id, fileName: "XML file name");

                // Assert
                var dataReturnUpload = domainHelper.GetDataReturnUpload(dataReturnUploadId);

                Assert.Equal(scheme.Id, dataReturnUpload.Scheme.Id);
                Assert.Equal("Test XML string", dataReturnUpload.RawData.Data);
                Assert.Equal("XML file name", dataReturnUpload.FileName);
                Assert.NotEqual(TimeSpan.Zero, dataReturnUpload.ProcessTime);
            }
        }

        [Fact]
        public async Task HandleAsync_XmlDoesNotContainSchemaError_StoresAvailableDataReturnData()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 3, false);

                database.Model.SaveChanges();

                var builder = new ProcessDataReturnXmlFileHandlerBuilder(database.WeeeContext);

                var schemeReturn = new SchemeReturn()
                {
                    ComplianceYear = "2016",
                    ReturnPeriod = SchemeReturnReturnPeriod.Quarter3JulySeptember
                };

                var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(
                    "Test XML string",
                    schemeReturn,
                    new List<XmlValidationError>());

                A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                     .Returns(xmlGeneratorResult);

                var dbDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion.Id);
                var dataReturnVersionBuilderResult = new DataReturnVersionBuilderResult(dbDataReturnVersion, A.Dummy<List<ErrorData>>());

                A.CallTo(() => builder.DataReturnVersionFromXmlBuilder.Build(A<SchemeReturn>._))
                    .Returns(dataReturnVersionBuilderResult);

                // Act
                var dataReturnUploadId = await builder.InvokeHandleAsync(organisation.Id, fileName: "XML file name");

                // Assert
                var dataReturnUpload = domainHelper.GetDataReturnUpload(dataReturnUploadId);

                Assert.Equal(scheme.Id, dataReturnUpload.Scheme.Id);
                Assert.Equal("Test XML string", dataReturnUpload.RawData.Data);
                Assert.Equal("XML file name", dataReturnUpload.FileName);
                Assert.Equal(2016, dataReturnUpload.ComplianceYear);
                Assert.Equal(Convert.ToInt32(QuarterType.Q3), dataReturnUpload.Quarter);
                Assert.NotEqual(TimeSpan.Zero, dataReturnUpload.ProcessTime);
            }
        }

        [Fact]
        public async Task HandleAsync_XmlContainsSchemaError_StoresSchemaErrors()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                database.Model.SaveChanges();

                var builder = new ProcessDataReturnXmlFileHandlerBuilder(database.WeeeContext);

                var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(
                    "Test XML string",
                    A.Dummy<SchemeReturn>(),
                    new List<XmlValidationError>
                    {
                        new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, "Error text1"),
                        new XmlValidationError(ErrorLevel.Error, XmlErrorType.Schema, "Error text2")
                    });

                A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                     .Returns(xmlGeneratorResult);

                // Act
                var dataReturnUploadId = await builder.InvokeHandleAsync(organisation.Id);

                // Assert
                var dataReturnUpload = domainHelper.GetDataReturnUpload(dataReturnUploadId);

                Assert.Equal(2, dataReturnUpload.Errors.Count);
                Assert.All(dataReturnUpload.Errors, e => Assert.Equal(Domain.Error.UploadErrorType.Schema, e.ErrorType));
                Assert.Contains(dataReturnUpload.Errors, p => p.ErrorLevel == Domain.Error.ErrorLevel.Error && p.Description == "Error text1");
                Assert.Contains(dataReturnUpload.Errors, p => p.ErrorLevel == Domain.Error.ErrorLevel.Error && p.Description == "Error text2");
            }
        }

        [Fact]
        public async Task HandleAsync_XmlContainsBusinessErrorAndWarning_StoresBusinessErrorAndWarning()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 3, false);

                database.Model.SaveChanges();

                var builder = new ProcessDataReturnXmlFileHandlerBuilder(database.WeeeContext);

                var schemeReturn = new SchemeReturn()
                {
                    ComplianceYear = "2016",
                    ReturnPeriod = SchemeReturnReturnPeriod.Quarter3JulySeptember
                };

                var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(
                    "Test XML string",
                    schemeReturn,
                    new List<XmlValidationError>());

                A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                     .Returns(xmlGeneratorResult);

                var dbDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion.Id);
                var businessErrorAndWarning = new List<ErrorData>
                {
                    new ErrorData("Error text", ErrorLevel.Error),
                    new ErrorData("Warning text", ErrorLevel.Warning)
                };

                var dataReturnVersionBuilderResult = new DataReturnVersionBuilderResult(dbDataReturnVersion, businessErrorAndWarning);

                A.CallTo(() => builder.DataReturnVersionFromXmlBuilder.Build(A<SchemeReturn>._))
                    .Returns(dataReturnVersionBuilderResult);

                // Act
                var dataReturnUploadId = await builder.InvokeHandleAsync(organisation.Id);

                // Assert
                var dataReturnUpload = domainHelper.GetDataReturnUpload(dataReturnUploadId);

                Assert.Equal(2, dataReturnUpload.Errors.Count);
                Assert.All(dataReturnUpload.Errors, e => Assert.Equal(Domain.Error.UploadErrorType.Business, e.ErrorType));
                Assert.Contains(dataReturnUpload.Errors, p => p.ErrorLevel == Domain.Error.ErrorLevel.Error && p.Description == "Error text");
                Assert.Contains(dataReturnUpload.Errors, p => p.ErrorLevel == Domain.Error.ErrorLevel.Warning && p.Description == "Warning text");
            }
        }

        [Fact]
        public async Task HandleAsync_XmlContainsError_DoesNotReturnVersionForDataUpload()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 3, false);

                database.Model.SaveChanges();

                var builder = new ProcessDataReturnXmlFileHandlerBuilder(database.WeeeContext);

                var schemeReturn = new SchemeReturn()
                {
                    ComplianceYear = "2016",
                    ReturnPeriod = SchemeReturnReturnPeriod.Quarter3JulySeptember
                };

                var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(
                    "Test XML string",
                    schemeReturn,
                    new List<XmlValidationError>());

                A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                     .Returns(xmlGeneratorResult);

                var dbDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion.Id);
                var businessErrorAndWarning = new List<ErrorData>
                {
                    new ErrorData("Error text", ErrorLevel.Error)
                };

                var dataReturnVersionBuilderResult = new DataReturnVersionBuilderResult(dbDataReturnVersion, businessErrorAndWarning);

                A.CallTo(() => builder.DataReturnVersionFromXmlBuilder.Build(A<SchemeReturn>._))
                    .Returns(dataReturnVersionBuilderResult);

                // Act
                var dataReturnUploadId = await builder.InvokeHandleAsync(organisation.Id);

                // Assert
                var dataReturnUpload = domainHelper.GetDataReturnUpload(dataReturnUploadId);

                Assert.Null(dataReturnUpload.DataReturnVersion);
            }
        }

        [Fact]
        public async Task HandleAsync_XmlDoesNotContainError_SetsReturnVersionForDataUpload()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);
                DomainHelper domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 3, false);

                database.Model.SaveChanges();

                var builder = new ProcessDataReturnXmlFileHandlerBuilder(database.WeeeContext);

                var schemeReturn = new SchemeReturn()
                {
                    ComplianceYear = "2016",
                    ReturnPeriod = SchemeReturnReturnPeriod.Quarter3JulySeptember
                };

                var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(
                    "Test XML string",
                    schemeReturn,
                    new List<XmlValidationError>());

                A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                     .Returns(xmlGeneratorResult);

                var dbDataReturnVersion = domainHelper.GetDataReturnVersion(dataReturnVersion.Id);

                var dataReturnVersionBuilderResult = new DataReturnVersionBuilderResult(dbDataReturnVersion, A.Dummy<List<ErrorData>>());

                A.CallTo(() => builder.DataReturnVersionFromXmlBuilder.Build(A<SchemeReturn>._))
                    .Returns(dataReturnVersionBuilderResult);

                // Act
                var dataReturnUploadId = await builder.InvokeHandleAsync(organisation.Id);

                // Assert
                var dataReturnUpload = domainHelper.GetDataReturnUpload(dataReturnUploadId);

                Assert.Equal(dbDataReturnVersion, dataReturnUpload.DataReturnVersion);
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
