namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using Requests.DataReturns;
    using Weee.Tests.Core;
    using Xml.Converter;
    using Xunit;

    public class ProcessDataReturnXMLFileHandlerTests
    {
        /// <summary>
        /// This test ensures that a user with no access to a scheme cannot create
        /// a data return.
        /// </summary>
        [Fact]
        public async void HandleAsync_UserNotAssociatedWithScheme_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .DenySchemeAccess()
                .Build();

            ProcessDataReturnXMLFileHandler handler = new ProcessDataReturnXMLFileHandler(
                                             A.Dummy<IProcessDataReturnXMLFileDataAccess>(),
                                             authorization,
                                             A.Dummy<IDataReturnXMLValidator>(),
                                             A.Dummy<IXmlConverter>(),
                                             A.Dummy<IGenerateFromDataReturnXML>());

            ProcessDataReturnXMLFile message = A.Dummy<ProcessDataReturnXMLFile>();
            // Act
            Func<Task<Guid>> testCode = async () => await handler.HandleAsync(message);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }
    }
}