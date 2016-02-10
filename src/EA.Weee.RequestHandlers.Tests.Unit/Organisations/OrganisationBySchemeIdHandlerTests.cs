namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Security;
    using DataAccess.DataAccess;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using Weee.Tests.Core;
    using Xunit;

    public class OrganisationBySchemeIdHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly IMapper mapper;

        public OrganisationBySchemeIdHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            mapper = A.Fake<IMapper>();
        }

        [Fact]
        public async void IfAUserDoesNotHaveAccessToEitherSchemeOrInternalArea_ASecurityExceptionIsThrown_AndNoDataIsRetrieved()
        {
            var message = new OrganisationBySchemeId(Guid.NewGuid());

            A.CallTo(() => authorization.CheckInternalOrSchemeAccess(message.SchemeId))
                .Throws<SecurityException>();

            await Assert.ThrowsAsync<SecurityException>(() => Handler().HandleAsync(message));

            A.CallTo(() => organisationDataAccess.GetBySchemeId(A<Guid>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task OrganisationBySchemeIdHandler_ReturnsTrueForCanEditOrganisation_WhenCurrentUserIsInternalAdmin()
        {
            // Arrange
            var weeeAuthorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .AllowRole(Roles.InternalAdmin)
                .Build();

            var handler = new OrganisationBySchemeIdHandler(weeeAuthorization, organisationDataAccess, mapper);

            var message = new OrganisationBySchemeId(Guid.NewGuid());
            
            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            Assert.True(result.CanEditOrganisation);
        }

        [Fact]
        public async Task OrganisationBySchemeIdHandler_ReturnsFalseForCanEditOrganisation_WhenCurrentUserIsNotInternalAdmin()
        {
            // Arrange
            var weeeAuthorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new OrganisationBySchemeIdHandler(weeeAuthorization, organisationDataAccess, mapper);

            var message = new OrganisationBySchemeId(Guid.NewGuid());

            // Act
            var result = await handler.HandleAsync(message);

            // Assert
            Assert.False(result.CanEditOrganisation);
        }

        private OrganisationBySchemeIdHandler Handler()
        {
            return new OrganisationBySchemeIdHandler(authorization, organisationDataAccess, mapper);
        }
    }
}
