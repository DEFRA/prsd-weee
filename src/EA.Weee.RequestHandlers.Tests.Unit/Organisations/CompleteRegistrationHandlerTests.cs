namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using DataAccess;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class CompleteRegistrationHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        private readonly IWeeeAuthorization permissiveAuthorization =
            AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

        private readonly IWeeeAuthorization denyingAuthorization =
            AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

        [Fact]
        public async Task CompleteRegistrationHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var handler = new CompleteRegistrationHandler(denyingAuthorization, A.Dummy<WeeeContext>());
            var message = new CompleteRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            await
                Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task CompleteRegistrationHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var handler = new CompleteRegistrationHandler(permissiveAuthorization, context);
            var message = new CompleteRegistration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var exception = await
                Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.Contains("COULD NOT FIND", exception.Message.ToUpperInvariant());
            Assert.Contains("ORGANISATION", exception.Message.ToUpperInvariant());
            Assert.Contains(message.OrganisationId.ToString(), exception.Message);
        }

        [Fact]
        public async Task CompleteRegistrationHandler_HappyPath_OrganisationCompleteAndSchemeAdded()
        {
            var context = A.Fake<WeeeContext>();

            var organisationId = Guid.NewGuid();
            var addressId = Guid.NewGuid();
            var contactId = Guid.NewGuid();

            var organisation = GetOrganisationWithId(organisationId);
            organisation.OrganisationStatus = OrganisationStatus.Incomplete;
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation> { organisation }));

            Scheme addedScheme = null;
            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Scheme>()));
            A.CallTo(() => context.Schemes.Add(A<Scheme>._))
                .Invokes((Scheme s) => addedScheme = s);

            var handler = new CompleteRegistrationHandler(permissiveAuthorization, context);
            var message = new CompleteRegistration(organisationId, addressId, contactId);

            var result = await handler.HandleAsync(message);

            Assert.Equal(organisationId, result);
            Assert.Equal(OrganisationStatus.Complete, organisation.OrganisationStatus);

            A.CallTo(() => context.Schemes.Add(A<Scheme>.That.Matches(s => s.AddressId.Equals(addressId) && s.ContactId.Equals(contactId)))).MustHaveHappened();
            Assert.NotNull(addedScheme);
            Assert.Equal(organisationId, addedScheme.OrganisationId);
        }

        private Organisation GetOrganisationWithId(Guid id)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(id);
            return organisation;
        }
    }
}
