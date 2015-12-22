namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Requests.DataReturns;
    using Weee.Tests.Core;
    using Xml.DataReturns;
    using XmlValidation.Errors;
    using Xunit;

    public class ProcessDataReturnXmlFileHandlerTests
    {
        /// <summary>
        /// This test ensures that a user with no access to a scheme cannot create
        /// a data return.
        /// </summary>
        [Fact]
        public async Task HandleAsync_UserNotAssociatedWithScheme_ThrowsSecurityException()
        {
            // Arrange
            var builder = new ProcessDataReturnXmlFileHandlerBuilder();
            builder.Authorization = new AuthorizationBuilder()
                .DenySchemeAccess()
                .Build();

            var handler = builder.Build();

            var message = A.Dummy<ProcessDataReturnXmlFile>();

            // Act
            Func<Task<Guid>> testCode = async () => await handler.HandleAsync(message);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task HandleAsync_SchemaErrors_DoesNotPerformBusinessValidation()
        {
            // Arrange
            var builder = new ProcessDataReturnXmlFileHandlerBuilder();

            var schemeReturn = new SchemeReturn() { ComplianceYear = "2016", ReturnPeriod = SchemeReturnReturnPeriod.Quarter1JanuaryMarch };
            var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(A.Dummy<string>(),
                schemeReturn, new List<XmlValidationError> { new XmlValidationError(Core.Shared.ErrorLevel.Error, XmlErrorType.Schema, "Test") });

            A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                .Returns(xmlGeneratorResult);

            // Act
            await builder.InvokeHandleAsync();

            // Assert
            A.CallTo(() => builder.DataReturnVersionFromXmlBuilder.Build(A<SchemeReturn>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_SavesDataUpload()
        {
            // Arrange
            var builder = new ProcessDataReturnXmlFileHandlerBuilder();

            var schemeReturn = new SchemeReturn() { ComplianceYear = "2016", ReturnPeriod = SchemeReturnReturnPeriod.Quarter1JanuaryMarch };
            var xmlGeneratorResult = new GenerateFromDataReturnXmlResult<SchemeReturn>(A.Dummy<string>(),
                schemeReturn, new List<XmlValidationError>());

            A.CallTo(() => builder.XmlGenerator.GenerateDataReturns<SchemeReturn>(A<ProcessDataReturnXmlFile>._))
                .Returns(xmlGeneratorResult);

            A.CallTo(() => builder.DataReturnVersionFromXmlBuilder.Build(A<SchemeReturn>._))
                 .Returns(new DataReturnVersionBuilderResult(A.Dummy<DataReturnVersion>(), A.Dummy<List<ErrorData>>()));

            // Act
            await builder.InvokeHandleAsync();

            // Assert
            A.CallTo(() => builder.DataAccess.AddAndSaveAsync(A<DataReturnUpload>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private class ProcessDataReturnXmlFileHandlerBuilder
        {
            public IProcessDataReturnXmlFileDataAccess DataAccess;
            public IWeeeAuthorization Authorization;
            public IGenerateFromDataReturnXml XmlGenerator;
            public IDataReturnVersionFromXmlBuilder DataReturnVersionFromXmlBuilder;
            private readonly Func<IDataReturnVersionBuilder, IDataReturnVersionFromXmlBuilder> dataReturnVersionFromXmlBuilderDelegate;
            private readonly Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilder> dataReturnVersionBuilderDelegate;

            public ProcessDataReturnXmlFileHandlerBuilder()
            {
                DataAccess = A.Fake<IProcessDataReturnXmlFileDataAccess>();
                Authorization = new AuthorizationBuilder()
                    .AllowEverything()
                    .Build();

                XmlGenerator = A.Fake<IGenerateFromDataReturnXml>();

                DataReturnVersionFromXmlBuilder = A.Fake<IDataReturnVersionFromXmlBuilder>();
                dataReturnVersionFromXmlBuilderDelegate = x => DataReturnVersionFromXmlBuilder;

                dataReturnVersionBuilderDelegate = A.Fake<Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilder>>();
            }

            public ProcessDataReturnXmlFileHandler Build()
            {
                return new ProcessDataReturnXmlFileHandler(
                                              DataAccess,
                                              Authorization,
                                              XmlGenerator,
                                              dataReturnVersionFromXmlBuilderDelegate,
                                              dataReturnVersionBuilderDelegate);
            }

            public Task InvokeHandleAsync(ProcessDataReturnXmlFile processDataReturnXmlFile = null)
            {
                var message = processDataReturnXmlFile ?? A.Dummy<ProcessDataReturnXmlFile>();

                return Build().HandleAsync(message);
            }
        }
    }
}