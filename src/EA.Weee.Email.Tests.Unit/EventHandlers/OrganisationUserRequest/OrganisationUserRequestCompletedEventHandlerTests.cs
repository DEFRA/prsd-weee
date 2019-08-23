namespace EA.Weee.Email.Tests.Unit.EventHandlers.OrganisationUserRequest
{
    using EA.Weee.Domain.Events;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Email.EventHandlers;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class OrganisationUserRequestCompletedEventHandlerTests
    {
        private IWeeeEmailService emailService;
        private readonly IOrganisationUserRequestEventHandlerDataAccess dataAccess;
        private OrganisationUserRequestCompletedEventHandler handler;

        public OrganisationUserRequestCompletedEventHandlerTests()
        {
            this.emailService = A.Fake<IWeeeEmailService>();
            this.dataAccess = A.Fake<IOrganisationUserRequestEventHandlerDataAccess>();

            this.handler = new OrganisationUserRequestCompletedEventHandler(dataAccess, emailService);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_DataAccessIsCalled()
        {
            var request = A.Dummy<OrganisationUserRequestCompletedEvent>();

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.FetchActiveOrganisationUsers(request.OrganisationUser.OrganisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_GivenRequest_SendOrganisationUserRequestCompletedIsCalledWithCorrectParameters(bool activeUsers)
        {
            var request = A.Dummy<OrganisationUserRequestCompletedEvent>();
            var activeOrganisationUsers = new List<OrganisationUser>();

            if (activeUsers)
            {
                activeOrganisationUsers.Add(A.Fake<OrganisationUser>());
            }

            A.CallTo(() => dataAccess.FetchActiveOrganisationUsers(request.OrganisationUser.OrganisationId)).Returns(activeOrganisationUsers);

            await handler.HandleAsync(request);

            A.CallTo(() => emailService.SendOrganisationUserRequestCompleted(request.OrganisationUser, activeUsers)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
