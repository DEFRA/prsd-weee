namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using RequestHandlers.Security;
    using Requests.DataReturns;
    using Weee.Tests.Core;
    using Xml.Converter;
    using Xunit;

    public class ProcessDataReturnXmlFileHandlerTests
    {
        private readonly IWeeeAuthorization permissiveAuthorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();
        private readonly IWeeeAuthorization denyingAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();     
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly ProcessDataReturnXmlFileHandler handler;
        private readonly IGenerateFromDataReturnXml generator;
        private readonly WeeeContext context;
        private readonly DbSet<Scheme> schemesDbSet;
        private readonly DbSet<DataReturnUpload> dataReturnUploadsDbSet;
        private readonly IDataReturnXmlValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private readonly IProcessDataReturnXmlFileDataAccess dataAccess;
        private static readonly Guid organisationId = Guid.NewGuid();
        private static readonly ProcessDataReturnXmlFile Message = new ProcessDataReturnXmlFile(organisationId, new byte[1], "File name");

        public ProcessDataReturnXmlFileHandlerTests()
        {
            dataReturnUploadsDbSet = A.Fake<DbSet<DataReturnUpload>>();
            xmlConverter = A.Fake<IXmlConverter>();
            var schemes = new[]
            {
                FakeSchemeData()
            };

            schemesDbSet = helper.GetAsyncEnabledDbSet(schemes);

            context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Schemes).Returns(schemesDbSet);
            A.CallTo(() => context.DataReturnsUploads).Returns(dataReturnUploadsDbSet);

            generator = A.Fake<IGenerateFromDataReturnXml>();
            xmlValidator = A.Fake<IDataReturnXmlValidator>();
            dataAccess = A.Fake<IProcessDataReturnXmlFileDataAccess>();            
          
            handler = new ProcessDataReturnXmlFileHandler(dataAccess, permissiveAuthorization, xmlValidator, xmlConverter, generator);
        }
        
        /// <summary>
        /// This test ensures that a user with no access to a scheme cannot create
        /// a data return.
        /// </summary>
        [Fact]
        public async Task HandleAsync_UserNotAssociatedWithScheme_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder()
                .DenySchemeAccess()
                .Build();

            ProcessDataReturnXmlFileHandler handler = new ProcessDataReturnXmlFileHandler(
                                             A.Dummy<IProcessDataReturnXmlFileDataAccess>(),
                                             authorization,
                                             A.Dummy<IDataReturnXmlValidator>(),
                                             A.Dummy<IXmlConverter>(),
                                             A.Dummy<IGenerateFromDataReturnXml>());

            ProcessDataReturnXmlFile message = A.Dummy<ProcessDataReturnXmlFile>();
            // Act
            Func<Task<Guid>> testCode = async () => await handler.HandleAsync(message);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(testCode);
        }

        [Fact]
        public async Task ProcessDataReturnXmlFile_SavesDataReturnUpload()
        {
            var id = await handler.HandleAsync(Message);
            Assert.NotNull(id);
        }

        [Fact]
        public async Task ProcessDataReturnXmlFile_StoresProcessTime()
        {
            IEnumerable<DataReturnUploadError> errors = new List<DataReturnUploadError>();
            Scheme scheme = FakeSchemeData();

            A.CallTo(() => xmlValidator.Validate(Message))
                .Returns(errors);

            DataReturnUpload upload = A.Fake<DataReturnUpload>();

            A.CallTo(() => generator.GenerateDataReturnsUpload(
                 Message,
                new List<DataReturnUploadError>(),
                scheme))
                 .WithAnyArguments()
                .Returns(upload);

            await handler.HandleAsync(Message);

            A.CallTo(() => upload.SetProcessTime(new TimeSpan()))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static Scheme FakeSchemeData()
        {
            return new Scheme(organisationId);
        }
    }    
}