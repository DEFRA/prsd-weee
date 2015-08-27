namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using Core.Helpers;
    using Core.Scheme;
    using DataAccess;
    using Domain.Scheme;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using Helpers;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.Scheme.MemberRegistration;
    using RequestHandlers.Security;
    using Requests.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetComplianceYearsHandlerTests
    {
        private readonly DbContextHelper dbContextHelper = new DbContextHelper();

        [Fact]
        public async Task GetComplianceYearsHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            // Arrange
            Guid pcsId = new Guid("A7905BCD-8EE7-48E5-9E71-2B571F7BBC81");

            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();

            GetComplianceYearsHandler handler = new GetComplianceYearsHandler(authorization, context);

            GetComplianceYears request = new GetComplianceYears(pcsId);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request); 

            // Asert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetComplianceYearsHandler_OrganisationDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            Guid pcsId = new Guid("A7905BCD-8EE7-48E5-9E71-2B571F7BBC81");

            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();
            
            WeeeContext context = A.Fake<WeeeContext>();
            var organisations = dbContextHelper.GetAsyncEnabledDbSet<Organisation>(new List<Organisation>());
            A.CallTo(() => organisations.FindAsync(pcsId)).Returns((Organisation)null);
            A.CallTo(() => context.Organisations).Returns(organisations);

            GetComplianceYearsHandler handler = new GetComplianceYearsHandler(authorization, context);

            GetComplianceYears request = new GetComplianceYears(pcsId);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Asert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}
