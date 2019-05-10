namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Charges;
    using Domain;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Specification;
    using RequestHandlers.Organisations;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;

    public class GenericDataAccessTests
    {
        [Fact]
        public async Task Add_EntityShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var countryId = new Guid("B5EBE1D1-8349-43CD-9E87-0081EA0A8463");
                var competantAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var competantAuthority = await competantAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = CreateContact(country);

                var aatf = CreateAatf(competantAuthority, database, contact);

                var result = await dataAccess.Add<Aatf>(aatf);

                result.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task AddMany_EntityShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                var originalAatfCount = database.WeeeContext.Aatfs.Count();
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var competantAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var @operator = new Operator(organisation);
                var competantAuthority = await competantAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = CreateContact(country);

                var aatf1 = new Aatf("Name1", competantAuthority, "approval1", AatfStatus.Approved, @operator, CreateAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact);
                var aatf2 = new Aatf("Name2", competantAuthority, "approval2", AatfStatus.Approved, @operator, CreateAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact);

                await dataAccess.AddMany<Aatf>(new List<Aatf>() { aatf1, aatf2 });
                var dbNewAatfs = database.WeeeContext.Aatfs.Count() - originalAatfCount;

                database.WeeeContext.Aatfs.Where(a => Equals(aatf1)).Should().NotBeNull();
                database.WeeeContext.Aatfs.Where(a => Equals(aatf2)).Should().NotBeNull();
                dbNewAatfs.Should().Be(2);
            }
        }

        [Fact]
        public async Task GetManyByExpression_GivenGetManyByExpressionSpecification_AatfsShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var @operator = new Operator(organisation);
                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = CreateContact(country);

                var aatf1 = new Aatf("Name1", competentAuthority, "approval1", AatfStatus.Approved, @operator, CreateAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact);
                var aatf2 = new Aatf("Name2", competentAuthority, "approval2", AatfStatus.Approved, @operator, CreateAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact);

                var organisation2 = Organisation.CreateSoleTrader("Test Organisation 2");
                var @operator2 = new Operator(organisation);
                var aatf3 = new Aatf("Name3", competentAuthority, "approval1", AatfStatus.Approved, @operator2, CreateAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact);

                database.WeeeContext.Aatfs.Add(aatf1);
                database.WeeeContext.Aatfs.Add(aatf2);

                await database.WeeeContext.SaveChangesAsync();

                var aatfInfo = await dataAccess.GetManyByExpression(new AatfsByOrganisationSpecification(organisation.Id));

                aatfInfo.Count.Should().Be(2);
                aatfInfo.Where(a => Equals(aatf1)).Should().NotBeNull();
                aatfInfo.Where(a => Equals(aatf2)).Should().NotBeNull();
                aatfInfo.Where(a => Equals(aatf3)).Should().BeEmpty();
            }
        }

        [Fact]
        public async Task GetAll_AllEntitiesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var count = database.WeeeContext.Aatfs.Count();

                var dataAccess = new GenericDataAccess(database.WeeeContext);

                var aatfInfo = await dataAccess.GetAll<Aatf>();

                aatfInfo.Count.Should().Be(count);
            }
        }

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority, DatabaseWrapper database, AatfContact contact)
        {
            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                new Operator(Organisation.CreatePartnership("trading")),
                CreateAddress(database), 
                A.Fake<AatfSize>(),
                DateTime.Now,
                contact);
        }

        private AatfAddress CreateAddress(DatabaseWrapper database)
        {
            var country = database.WeeeContext.Countries.First();

            return new AatfAddress("name", "one", "two", "bath", "BANES", "BA2 2PL", country);
        }

        private AatfContact CreateContact(Domain.Country country)
        {
            return new AatfContact("First Name",
                "Last Name",
                "Manager",
                "1 Address Lane",
                "Address Ward",
                "Town",
                "County",
                "Postcode",
                country,
                "01234 567890",
                "email@email.com");
        }
    }
}
