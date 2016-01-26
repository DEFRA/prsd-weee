namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.Create
{
    using System;
    using System.Data.Entity;
    using DataAccess;
    using Domain.Organisation;
    using Domain.User;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class CreateOrganisationRequestHandlerTestsBase
    {
        protected IWeeeAuthorization permissiveAuthorization = new AuthorizationBuilder().AllowExternalAreaAccess().Build();
        protected IWeeeAuthorization denyingAuthorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

        private readonly DbContextHelper dbHelper = new DbContextHelper();

        protected Organisation addedOrganisation;
        protected Guid addedOrganisationId;
        protected OrganisationUser addedOrganisationUser;

        protected WeeeContext GetPreparedContext()
        {
            WeeeContext context = A.Fake<WeeeContext>();

            var organisations = A.Fake<DbSet<Organisation>>();
            A.CallTo(() => organisations.Add(A<Organisation>._)).Invokes((Organisation o) =>
            {
                addedOrganisation = o;
                addedOrganisationId = Guid.NewGuid();
                dbHelper.SetId(o, addedOrganisationId);
            });

            var organisationUsers = A.Fake<DbSet<OrganisationUser>>();
            A.CallTo(() => organisationUsers.Add(A<OrganisationUser>._)).Invokes((OrganisationUser ou) => addedOrganisationUser = ou);

            A.CallTo(() => context.Organisations).Returns(organisations);
            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            return context;
        }

        protected IUserContext GetPreparedUserContext()
        {
            var userId = Guid.NewGuid();
            var userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId).Returns(userId);
            return userContext;
        }

        protected void DoSharedAssertions(string tradingName)
        {
            Assert.NotNull(addedOrganisation);
            Assert.NotNull(addedOrganisationUser);
            Assert.NotEqual(Guid.Empty, addedOrganisationId);
            Assert.Equal(tradingName, addedOrganisation.TradingName);
            Assert.Equal(addedOrganisationId, addedOrganisationUser.OrganisationId);
            Assert.Equal(UserStatus.Active, addedOrganisationUser.UserStatus);
        }
    }
}
