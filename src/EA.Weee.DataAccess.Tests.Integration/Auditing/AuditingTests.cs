namespace EA.Weee.DataAccess.Tests.Integration.Auditing
{
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Xunit;

    public class AuditingTests
    {
        private readonly IUserContext userContext;

        public AuditingTests()
        {
            userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId)
                .Returns(Guid.NewGuid());
        }

        [Fact]
        public void AddressAddedToExistingOrganisation_OrganisationUpdateIsAudited_AddressCreateIsAudited()
        {
            // Arrange
            var organisation = Organisation.CreateSoleTrader("test name");

            var context = WeeeContext();
            organisation = context.Organisations.Add(organisation);
            context.SaveChanges(); // This will reset the change tracker

            // Act
            organisation.AddOrUpdateAddress(AddressType.OrganisationAddress, ValidAddress());

            var organisationChanges = context.ChangeTracker.Entries<Organisation>();
            var addressChanges = context.ChangeTracker.Entries<Address>();

            Assert.Equal(EntityState.Added, addressChanges.Single().State);
            Assert.Equal(EntityState.Modified, organisationChanges.Single().State);
        }

        private WeeeContext WeeeContext()
        {
            IEventDispatcher eventDispatcher = A.Fake<IEventDispatcher>();

            return new WeeeContext(userContext, eventDispatcher);
        }

        private Address ValidAddress()
        {
            return new Address("a", "b", "c", "d", "e", new Country(Guid.NewGuid(), "f"), "g", "h");
        }
    }
}
