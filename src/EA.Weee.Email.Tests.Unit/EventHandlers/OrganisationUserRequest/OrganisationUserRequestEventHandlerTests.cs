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
    using Domain;
    using Xunit;

    public class OrganisationUserRequestEventHandlerTests
    {
        private readonly IOrganisationUserRequestEventHandlerDataAccess dataAccess;
        private readonly IWeeeEmailService emailService;
        private readonly OrganisationUserRequestEventHandler handler;

        public OrganisationUserRequestEventHandlerTests()
        {
            dataAccess = A.Fake<IOrganisationUserRequestEventHandlerDataAccess>();
            emailService = A.Fake<IWeeeEmailService>();

            handler = new OrganisationUserRequestEventHandler(dataAccess, emailService);
        }

        [Fact]
        public async Task HandleAsync_GivenNoActiveUsers_SendOrganisationRequestToEA()
        {
            var request = new OrganisationUserRequestEvent(Guid.NewGuid(), Guid.NewGuid());

            var organisation = Organisation.CreateRegisteredCompany("company", "12345678");
            var activeUsersFalse = new List<OrganisationUser>();
            var competentAuthorities = new List<UKCompetentAuthority>()
            {
                new UKCompetentAuthority(Guid.NewGuid(), "name", "abb", A.Fake<Country>(), "email1", null),
                new UKCompetentAuthority(Guid.NewGuid(), "name", "abb", A.Fake<Country>(), "email2", null)
            };

            A.CallTo(() => dataAccess.FetchCompetentAuthorities()).Returns(competentAuthorities);
            A.CallTo(() => dataAccess.FetchActiveOrganisationUsers(request.OrganisationId)).Returns(activeUsersFalse);
            A.CallTo(() => dataAccess.FetchOrganisation(A<Guid>._)).Returns(organisation);

            await handler.HandleAsync(request);

            A.CallTo(() => emailService.SendOrganisationUserRequestToEA("email1", organisation.Name, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => emailService.SendOrganisationUserRequestToEA("email2", organisation.Name, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenActiveUsers_SendOrganisationRequestToAdministrator()
        {
            var request = new OrganisationUserRequestEvent(Guid.NewGuid(), Guid.NewGuid());

            var activeUsersTrue = new List<OrganisationUser>()
            {
                A.Fake<OrganisationUser>()
            };

            A.CallTo(() => dataAccess.FetchActiveOrganisationUsers(request.OrganisationId)).Returns(activeUsersTrue);

            await handler.HandleAsync(request);

            A.CallTo(() => emailService.SendOrganisationUserRequest(A<string>._, A<string>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationId_FetchOrganisationIsRunWithId()
        {
            var request = new OrganisationUserRequestEvent(Guid.NewGuid(), Guid.NewGuid());

            await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.FetchOrganisation(request.OrganisationId)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
