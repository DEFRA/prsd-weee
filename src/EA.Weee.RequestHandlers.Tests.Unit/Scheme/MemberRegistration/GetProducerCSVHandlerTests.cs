namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.Scheme;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using Requests.Scheme.MemberRegistration;
    using Weee.Tests.Core;
    using Xunit;

    public class GetProducerCSVHandlerTests
    {
        private readonly DbContextHelper dbContextHelper = new DbContextHelper();

        [Fact]
        public async Task GetProducerCSVHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            // Arrange
            Guid pcsId = new Guid("62874744-6F52-4311-B4C0-3DD7767BEBF6");
            int complianceYear = 2016;

            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalOrOrganisationAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetProducerCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerCSV(pcsId, complianceYear);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetProducerCSVHandler_OrganisationDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            Guid pcsId = new Guid("62874744-6F52-4311-B4C0-3DD7767BEBF6");
            int complianceYear = 2016;

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            WeeeContext context = A.Fake<WeeeContext>();
            var organisations = dbContextHelper.GetAsyncEnabledDbSet<Organisation>(new List<Organisation>());
            A.CallTo(() => organisations.FindAsync(pcsId)).Returns((Organisation)null);
            A.CallTo(() => context.Organisations).Returns(organisations);

            var schemes = dbContextHelper.GetAsyncEnabledDbSet<Scheme>(new List<Scheme>
            {
                new Scheme(pcsId)
            });
            A.CallTo(() => context.Schemes).Returns(schemes);

            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetProducerCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerCSV(pcsId, complianceYear);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
