namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
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
        public async void HandleAsync_UserNotAssociatedWithScheme_ThrowsSecurityException()
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
        public async void HandleAsync_SchemaErrors_DoesNotPerformBusinessValidation()
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
            A.CallTo(() => builder.XmlBusinessValidator.Build(A<SchemeReturn>._)).MustNotHaveHappened();
        }

        private class ProcessDataReturnXmlFileHandlerBuilder
        {
            public IProcessDataReturnXmlFileDataAccess DataAccess;
            public IWeeeAuthorization Authorization;
            public IGenerateFromDataReturnXml XmlGenerator;
            public IDataReturnVersionFromXmlBuilder XmlBusinessValidator;
            public Func<IDataReturnVersionBuilder, IDataReturnVersionFromXmlBuilder> XmlBusinessValidatorDelegate;
            public Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilder> DataReturnVersionBuilderDelegate;

            public ProcessDataReturnXmlFileHandlerBuilder()
            {
                DataAccess = A.Fake<IProcessDataReturnXmlFileDataAccess>();
                Authorization = new AuthorizationBuilder()
                    .AllowEverything()
                    .Build();

                XmlGenerator = A.Fake<IGenerateFromDataReturnXml>();

                XmlBusinessValidator = A.Fake<IDataReturnVersionFromXmlBuilder>();
                XmlBusinessValidatorDelegate = new Func<IDataReturnVersionBuilder, IDataReturnVersionFromXmlBuilder>(x => XmlBusinessValidator);

                DataReturnVersionBuilderDelegate = A.Fake<Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilder>>();
            }

            public ProcessDataReturnXmlFileHandler Build()
            {
                return new ProcessDataReturnXmlFileHandler(
                                              DataAccess,
                                              Authorization,
                                              XmlGenerator,
                                              XmlBusinessValidatorDelegate,
                                              DataReturnVersionBuilderDelegate);
            }

            public Task InvokeHandleAsync(ProcessDataReturnXmlFile processDataReturnXmlFile = null)
            {
                var message = processDataReturnXmlFile ?? A.Dummy<ProcessDataReturnXmlFile>();

                return Build().HandleAsync(message);
            }
        }
    }
}