namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations.Create
{
    using System;
    using System.Data.Entity;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core.Domain;

    public class CreateOrganisationRequestHandlerTestsBase
    {
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
                AddId(o, addedOrganisationId);
            });

            var organisationUsers = A.Fake<DbSet<OrganisationUser>>();
            A.CallTo(() => organisationUsers.Add(A<OrganisationUser>._)).Invokes((OrganisationUser ou) => addedOrganisationUser = ou);

            A.CallTo(() => context.Organisations).Returns(organisations);
            A.CallTo(() => context.OrganisationUsers).Returns(organisationUsers);

            return context;
        }

        private void AddId(Entity entity, Guid id)
        {
            typeof(Entity).GetProperty("Id").SetValue(entity, id);
        }
    }
}
