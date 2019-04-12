namespace EA.Weee.DataAccess.Tests.Integration.Auditing
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Domain;
    using Domain.Organisation;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Address = Domain.Organisation.Address;
    using Contact = Domain.Organisation.Contact;
    using Country = Domain.Country;
    using Organisation = Domain.Organisation.Organisation;
    using Scheme = Domain.Scheme.Scheme;

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
        public void AddressAddedToExistingScheme_SchemeUpdateIsAudited_AddressCreateIsAudited()
        {   
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var organisation = Organisation.CreateSoleTrader("test name");
                var scheme = new Scheme(organisation);

                organisation = context.Organisations.Add(organisation);
                scheme = context.Schemes.Add(scheme);

                context.SaveChanges(); // This will reset the change tracker

                scheme.AddOrUpdateAddress(ValidAddress());
                
                var schemeChanges = context.ChangeTracker.Entries<Scheme>();
                var addressChanges = context.ChangeTracker.Entries<Address>();

                Assert.Equal(EntityState.Added, addressChanges.Single().State);
                Assert.Equal(EntityState.Modified, schemeChanges.Single().State);
            }
        }

        [Fact]
        public void ContactAddedToExistingScheme_SchemeUpdateIsAudited_ContactCreateIsAudited()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var organisation = Organisation.CreateSoleTrader("test name");
                var scheme = new Scheme(organisation);

                organisation = context.Organisations.Add(organisation);
                scheme = context.Schemes.Add(scheme);

                context.SaveChanges(); // This will reset the change tracker

                scheme.AddOrUpdateMainContactPerson(new Contact("first", "last", "position"));

                var schemeChanges = context.ChangeTracker.Entries<Scheme>();
                var contactChanges = context.ChangeTracker.Entries<Contact>();

                Assert.Equal(EntityState.Added, contactChanges.Single().State);
                Assert.Equal(EntityState.Modified, schemeChanges.Single().State);
            }
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
