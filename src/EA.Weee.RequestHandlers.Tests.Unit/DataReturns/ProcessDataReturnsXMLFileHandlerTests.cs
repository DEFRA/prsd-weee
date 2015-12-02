namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Security;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using RequestHandlers.DataReturns;
    using RequestHandlers.DataReturns.GenerateDomainObjects;
    using RequestHandlers.DataReturns.XmlValidation;
    using Requests.DataReturns;
    using Weee.Tests.Core;
    using Xml.Converter;
    using Xunit;

    public class ProcessDataReturnsXMLFileHandlerTests
    {
        private readonly IWeeeAuthorization permissiveAuthorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();
        private readonly IWeeeAuthorization denyingAuthorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly ProcessDataReturnsXMLFileHandler handler;
        private readonly IGenerateFromDataReturnsXML generator;
        private readonly WeeeContext context;
        private readonly DbSet<Scheme> schemesDbSet;
        private readonly DbSet<DataReturnsUpload> datareturnsUploadsDbSet;
        private readonly IDataReturnsXMLValidator xmlValidator;
        private readonly IXmlConverter xmlConverter;
        private static readonly Guid organisationId = Guid.NewGuid();
        private static readonly ProcessDataReturnsXMLFile Message = new ProcessDataReturnsXMLFile(organisationId, new byte[1], "File name");

        public ProcessDataReturnsXMLFileHandlerTests()
        {
            datareturnsUploadsDbSet = A.Fake<DbSet<DataReturnsUpload>>();
            xmlConverter = A.Fake<IXmlConverter>();
            var schemes = new[]
            {
                FakeSchemeData()
            };

            schemesDbSet = helper.GetAsyncEnabledDbSet(schemes);

            context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Schemes).Returns(schemesDbSet);
            A.CallTo(() => context.DataReturnsUploads).Returns(datareturnsUploadsDbSet);

            generator = A.Fake<IGenerateFromDataReturnsXML>();
            xmlValidator = A.Fake<IDataReturnsXMLValidator>();
         
            handler = new ProcessDataReturnsXMLFileHandler(context, permissiveAuthorization, xmlValidator, xmlConverter, generator);
        }

        [Fact]
        public async void NotOrganisationUser_ThrowsSecurityException()
        {
            var authorisationDeniedHandler = new ProcessDataReturnsXMLFileHandler(context, denyingAuthorization, xmlValidator, xmlConverter, generator);

            await
                Assert.ThrowsAsync<SecurityException>(
                    async () => await authorisationDeniedHandler.HandleAsync(Message));
        }

        [Fact]
        public async void ProcessDataReturnsXMLFile_SavesDataReturnsUpload()
        {
            var id = await handler.HandleAsync(Message);
            Assert.NotNull(id);
        }
        
        public static Scheme FakeSchemeData()
        {
            return new Scheme(organisationId);
        }
    }
}
