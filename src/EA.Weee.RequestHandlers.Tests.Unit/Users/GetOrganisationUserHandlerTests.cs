namespace EA.Weee.RequestHandlers.Tests.Unit.Users
{
    using DataAccess;
    using Domain.Organisation;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using RequestHandlers.Users;
    using Requests.Users;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetOrganisationUserHandlerTests
    {
        [Fact]
        public async Task HandleAsync_HappyPath_ReturnsOrganisationUserData()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Guid organisationUserId = new Guid("10C57182-BF30-4729-BBF8-F8BCBC00EB77");
            OrganisationUser organisationUser = A.Fake<OrganisationUser>();

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.OrganisationUsers.FindAsync(organisationUserId)).Returns(organisationUser);

            IMap<OrganisationUser, OrganisationUserData> mapper = A.Fake<IMap<OrganisationUser, OrganisationUserData>>();
            OrganisationUserData organisationUserData = new OrganisationUserData();
            A.CallTo(() => mapper.Map(organisationUser)).Returns(organisationUserData);

            var handler = new GetOrganisationUserHandler(context, authorization, mapper);
            var request = new GetOrganisationUser(organisationUserId);

            // Act
            OrganisationUserData result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(organisationUserData, result);
        }

        [Fact]
        [Trait("Authorization", "OrganisationAccess")]
        public async Task HandleAsync_NotOrganisationUser_ThrowsSecurityException()
        {
            // Arrange
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            Guid organisationUserId = new Guid("10C57182-BF30-4729-BBF8-F8BCBC00EB77");
            OrganisationUser organisationUser = A.Fake<OrganisationUser>();

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.OrganisationUsers.FindAsync(organisationUserId)).Returns(organisationUser);

            IMap<OrganisationUser, OrganisationUserData> mapper = A.Fake<IMap<OrganisationUser, OrganisationUserData>>();
            OrganisationUserData organisationUserData = new OrganisationUserData();
            A.CallTo(() => mapper.Map(organisationUser)).Returns(organisationUserData);

            var handler = new GetOrganisationUserHandler(context, authorization, mapper);
            var request = new GetOrganisationUser(organisationUserId);
            
            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithUnknownOrganisationUserId_ThrowsException()
        {
            // Arrange
            IWeeeAuthorization authorization = AuthorizationBuilder.CreateUserWithAllRights();

            Guid organisationUserId = new Guid("10C57182-BF30-4729-BBF8-F8BCBC00EB77");

            WeeeContext context = A.Fake<WeeeContext>();
            A.CallTo(() => context.OrganisationUsers.FindAsync(organisationUserId)).Returns((OrganisationUser)null);

            IMap<OrganisationUser, OrganisationUserData> mapper = A.Fake<IMap<OrganisationUser, OrganisationUserData>>();

            var handler = new GetOrganisationUserHandler(context, authorization, mapper);
            var request = new GetOrganisationUser(organisationUserId);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<Exception>(action);
        }
    }
}
