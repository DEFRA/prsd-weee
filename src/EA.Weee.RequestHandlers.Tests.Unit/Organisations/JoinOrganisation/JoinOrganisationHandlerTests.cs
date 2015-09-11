namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.JoinOrganisation
{
    using System;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Organisations.JoinOrganisation;
    using RequestHandlers.Organisations.JoinOrganisation.DataAccess;
    using RequestHandlers.Security;
    using Requests.Organisations;
    using Xunit;

    public class JoinOrganisationHandlerTests
    {
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IUserContext userContext;
        private readonly IJoinOrganisationDataAccess dataAccess;

        public JoinOrganisationHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            userContext = A.Fake<IUserContext>();
            dataAccess = A.Fake<IJoinOrganisationDataAccess>();
        }

        [Fact]
        public async Task JoinOrganisationHandler_UserDoesNotExist_ShouldVerifyAuthorization_AndThrowArgumentException()
        {
            A.CallTo(() => userContext.UserId)
                .Returns(Guid.NewGuid());

            A.CallTo(() => dataAccess.DoesUserExist(A<Guid>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<ArgumentException>(
                () => JoinOrganisationHandler().HandleAsync(new JoinOrganisation(Guid.NewGuid())));

            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea())
                .MustHaveHappened();
        }

        [Fact]
        public async Task JoinOrganisationHandler_UserDoesExist_ButOrganisationDoesNotExist_ShouldVerifyAuthorization_AndThrowArgumentException()
        {
            A.CallTo(() => userContext.UserId)
                .Returns(Guid.NewGuid());

            A.CallTo(() => dataAccess.DoesUserExist(A<Guid>._))
                .Returns(true);

            A.CallTo(() => dataAccess.DoesOrganisationExist(A<Guid>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<ArgumentException>(
                () => JoinOrganisationHandler().HandleAsync(new JoinOrganisation(Guid.NewGuid())));

            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea())
                .MustHaveHappened();
        }

        [Fact]
        public async Task JoinOrganisationHandler_UserAndOrganisationExist_ButResultIsUnsuccessful_ShouldVerifyAuthorization_AndShouldThrowInvalidOperationException()
        {
            A.CallTo(() => userContext.UserId)
                .Returns(Guid.NewGuid());

            A.CallTo(() => dataAccess.DoesUserExist(A<Guid>._))
                .Returns(true);

            A.CallTo(() => dataAccess.DoesOrganisationExist(A<Guid>._))
                .Returns(true);

            A.CallTo(() => dataAccess.JoinOrganisation(A<OrganisationUser>._))
                .Returns(JoinOrganisationResult.Fail("Something went wrong"));

            await Assert.ThrowsAnyAsync<InvalidOperationException>(
                () => JoinOrganisationHandler().HandleAsync(new JoinOrganisation(Guid.NewGuid())));

            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea())
                .MustHaveHappened();
        }

        [Fact]
        public async Task JoinOrganisationHandler_UserAndOrganisationExist_AndResultIsSuccessful_ShouldVerifyAuthorization_AndShouldReturnOrganisationId()
        {
            var organisationId = Guid.NewGuid();

            A.CallTo(() => userContext.UserId)
                .Returns(Guid.NewGuid());

            A.CallTo(() => dataAccess.DoesUserExist(A<Guid>._))
                .Returns(true);

            A.CallTo(() => dataAccess.DoesOrganisationExist(A<Guid>._))
                .Returns(true);

            A.CallTo(() => dataAccess.JoinOrganisation(A<OrganisationUser>._))
                .Returns(JoinOrganisationResult.Success());

            var result = await JoinOrganisationHandler().HandleAsync(new JoinOrganisation(organisationId));

            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea())
                .MustHaveHappened();

            Assert.Equal(organisationId, result);
        }

        private JoinOrganisationHandler JoinOrganisationHandler()
        {
            return new JoinOrganisationHandler(weeeAuthorization, dataAccess, userContext);
        }
    }
}
