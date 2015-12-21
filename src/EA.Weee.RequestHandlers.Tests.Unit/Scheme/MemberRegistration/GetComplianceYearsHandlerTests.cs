namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Weee.Domain.Organisation;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration;
    using RequestHandlers.Security;
    using Requests.Scheme.MemberRegistration;
    using Weee.Tests.Core;
    using Xunit;

    public class GetComplianceYearsHandlerTests
    {
        private readonly DbContextHelper dbContextHelper = new DbContextHelper();

        [Fact]
        public async Task GetComplianceYearsHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            // Arrange
            Guid pcsId = new Guid("A7905BCD-8EE7-48E5-9E71-2B571F7BBC81");

            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalOrOrganisationAccess().Build();
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
