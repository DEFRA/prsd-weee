namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.GetActiveOrgnisationUsers
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.User;
    using EA.Weee.RequestHandlers.Organisations.GetActiveOrganisationUsers;
    using EA.Weee.RequestHandlers.Organisations.GetActiveOrganisationUsers.DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GetActiveOrganisationUsersHandlerTests
    {
        private readonly IGetActiveOrganisationUsersDataAccess dataAccess;
        private readonly IMap<OrganisationUser, OrganisationUserData> mapper;
        private readonly GetActiveOrganisationUsersHandler handler;

        public GetActiveOrganisationUsersHandlerTests()
        {
            dataAccess = A.Fake<IGetActiveOrganisationUsersDataAccess>();
            mapper = A.Fake<IMap<OrganisationUser, OrganisationUserData>>();

            handler = new GetActiveOrganisationUsersHandler(dataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationId_FetchActiveOrganisationUsersIsCalledWithId()
        {
            var request = new GetActiveOrganisationUsers(Guid.NewGuid());

            var result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.FetchActiveOrganisationUsers(request.OrganisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenListOfUsers_MapperIsCalledForEveryUser()
        {
            var request = new GetActiveOrganisationUsers(Guid.NewGuid());

            var users = new List<OrganisationUser>()
            {
                new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Active),
                new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Inactive)
            };

            A.CallTo(() => dataAccess.FetchActiveOrganisationUsers(request.OrganisationId)).Returns(users);

            var result = await handler.HandleAsync(request);

            foreach (var user in users)
            {
                A.CallTo(() => mapper.Map(user)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_GivenUsers_UserIsMapped()
        {
            var request = new GetActiveOrganisationUsers(Guid.NewGuid());

            var user = new OrganisationUser(Guid.NewGuid(), Guid.NewGuid(), UserStatus.Active);

            var users = new List<OrganisationUser>()
            {
                user
            };

            var userMapped = new OrganisationUserData()
            {
                UserId = user.Id.ToString(),
                OrganisationId = user.OrganisationId,
                UserStatus = (Core.Shared.UserStatus)Enum.Parse(typeof(Core.Shared.UserStatus), user.UserStatus.Value.ToString())
            };

            A.CallTo(() => dataAccess.FetchActiveOrganisationUsers(request.OrganisationId)).Returns(users);
            A.CallTo(() => mapper.Map(user)).Returns(userMapped);

            var result = await handler.HandleAsync(request);

            result.Should().Contain(userMapped);
        }
    }
}
