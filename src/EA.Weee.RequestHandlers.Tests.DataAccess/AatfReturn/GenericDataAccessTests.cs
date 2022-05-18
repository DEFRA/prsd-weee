namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Specification;
    using RequestHandlers.Shared;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class GenericDataAccessTests
    {
        [Fact]
        public async Task Add_EntityShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenericDataAccess(database.WeeeContext);

                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                var result = await dataAccess.Add<Aatf>(aatf);

                result.Should().Be(aatf);
            }
        }

        [Fact]
        public async Task AddMany_EntityShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                var originalAatfCount = database.WeeeContext.Aatfs.Count();
                var dataAccess = new GenericDataAccess(database.WeeeContext);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

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

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                var organisation2 = Organisation.CreateSoleTrader("Test Organisation 2");
                var aatf3 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation2);

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
        public async Task GetManyByExpression_Specification_AatfsByAatfFacilityTypeShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenericDataAccess(database.WeeeContext);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                var aatf3 = new Aatf("aatfname", database.WeeeContext.UKCompetentAuthorities.First(), "number", AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(database), AatfSize.Large, DateTime.Now, ObligatedWeeeIntegrationCommon.CreateDefaultContact(database.WeeeContext.Countries.First()), FacilityType.Ae, 2019, database.WeeeContext.LocalAreas.First(), database.WeeeContext.PanAreas.First());

                database.WeeeContext.Aatfs.Add(aatf1);
                database.WeeeContext.Aatfs.Add(aatf2);
                database.WeeeContext.Aatfs.Add(aatf3);
                await database.WeeeContext.SaveChangesAsync();

                var aatfInfo = await dataAccess.GetManyByExpression(new AatfsByOrganisationAndFacilityTypeSpecification(organisation.Id, Core.AatfReturn.FacilityType.Aatf));

                aatfInfo.Count.Should().Be(2);
                aatfInfo.Where(a => Equals(aatf1)).Should().NotBeNull();
                aatfInfo.Where(a => Equals(aatf2)).Should().NotBeNull();
                aatfInfo.Where(a => Equals(aatf3)).Should().BeEmpty();
            }
        }

        [Fact]
        public async Task GetManyByExpression_Specification_AatfsByAeFacilityTypeShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenericDataAccess(database.WeeeContext);

                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                var aatf3 = new Aatf("aatfname", database.WeeeContext.UKCompetentAuthorities.First(), "number", AatfStatus.Approved, organisation, ObligatedWeeeIntegrationCommon.CreateAatfAddress(database), AatfSize.Large, DateTime.Now, ObligatedWeeeIntegrationCommon.CreateDefaultContact(database.WeeeContext.Countries.First()), FacilityType.Ae, 2019, database.WeeeContext.LocalAreas.First(), database.WeeeContext.PanAreas.First());

                database.WeeeContext.Aatfs.Add(aatf1);
                database.WeeeContext.Aatfs.Add(aatf2);
                database.WeeeContext.Aatfs.Add(aatf3);
                await database.WeeeContext.SaveChangesAsync();

                var aatfInfo = await dataAccess.GetManyByExpression(new AatfsByOrganisationAndFacilityTypeSpecification(organisation.Id, Core.AatfReturn.FacilityType.Ae));

                aatfInfo.Count.Should().Be(1);
                aatfInfo.Where(a => Equals(aatf1)).Should().NotBeNull();
                aatfInfo.Where(a => Equals(aatf2)).Should().BeEmpty();
                aatfInfo.Where(a => Equals(aatf3)).Should().BeEmpty();
            }
        }

        [Fact]
        public async Task GetAll_AllEntitiesShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var organisation = Organisation.CreateSoleTrader("Test Organisation");

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                await dataAccess.AddMany<Aatf>(new List<Aatf>() { aatf1, aatf2 });

                await database.WeeeContext.SaveChangesAsync();

                var count = database.WeeeContext.Aatfs.Count();

                var aatfInfo = await dataAccess.GetAll<Aatf>();

                aatfInfo.ToList().Count.Should().Be(count);
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
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation);

                database.WeeeContext.Aatfs.Add(ObligatedWeeeIntegrationCommon.CreateAatf(database, organisation));

                await database.WeeeContext.SaveChangesAsync();

                dataAccess.Remove<Aatf>(null);

                database.WeeeContext.ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted).Should().Be(0);
            }
        }
    }
}
