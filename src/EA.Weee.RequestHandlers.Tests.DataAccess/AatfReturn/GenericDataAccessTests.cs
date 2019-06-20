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
    using Organisation = Domain.Organisation.Organisation;

    public class GenericDataAccessTests
    {
        [Fact]
        public async Task Add_EntityShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var countryId = new Guid("B5EBE1D1-8349-43CD-9E87-0081EA0A8463");
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = CreateContact(country);

                var aatf = CreateAatf(competentAuthority, database, contact);

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
                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = CreateContact(country);

                var aatf1 = new Aatf("Name1", competentAuthority, "approval1", AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, 2019);
                var aatf2 = new Aatf("Name2", competentAuthority, "approval2", AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, 2019);

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
                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = CreateContact(country);

                var aatf1 = new Aatf("Name1", competentAuthority, "approval1", AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, 2019);
                var aatf2 = new Aatf("Name2", competentAuthority, "approval2", AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, 2019);

                var organisation2 = Organisation.CreateSoleTrader("Test Organisation 2");
                var aatf3 = new Aatf("Name3", competentAuthority, "approval1", AatfStatus.Approved, organisation2, AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, 2019);

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
               var dataAccess = new GenericDataAccess(database.WeeeContext);
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = CreateContact(country);

                var aatf1 = new Aatf("Name1", competentAuthority, "approval1", AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, (Int16)2019);
                var aatf2 = new Aatf("Name2", competentAuthority, "approval2", AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, (Int16)2019);

                await dataAccess.AddMany<Aatf>(new List<Aatf>() { aatf1, aatf2 });

                await database.WeeeContext.SaveChangesAsync();

                var count = database.WeeeContext.Aatfs.Count();

                var aatfInfo = await dataAccess.GetAll<Aatf>();

                aatfInfo.Count.Should().Be(count);
            }
        }

        [Fact]
        public async Task Remove_GivenNullEntity_EntityStateShouldNotBeUpdated()
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var contact = CreateContact(country);

                database.WeeeContext.Aatfs.Add(new Aatf("Name1", competentAuthority, "approval1", AatfStatus.Approved, organisation,
                    AddressHelper.GetAatfAddress(database), A.Fake<AatfSize>(), DateTime.Now, contact, FacilityType.Aatf, (Int16)2019));

                await database.WeeeContext.SaveChangesAsync();

                dataAccess.Remove<Aatf>(null);

                database.WeeeContext.ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted).Should().Be(0);
            }
        }

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority, DatabaseWrapper database, AatfContact contact)
        {
            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                Organisation.CreatePartnership("trading"),
                AddressHelper.GetAatfAddress(database), 
                A.Fake<AatfSize>(),
                DateTime.Now,
                contact,
                FacilityType.Aatf,
                2019);
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
