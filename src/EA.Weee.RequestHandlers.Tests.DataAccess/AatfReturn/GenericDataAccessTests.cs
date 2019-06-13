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
    using RequestHandlers.Shared;
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
                
                var organisation = ObligatedWeeeIntegrationCommon.CreateOrganisation();
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation);

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
                
                var organisation = Organisation.CreateSoleTrader("Test Organisation");
                
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation);

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
                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation);

                var organisation2 = Organisation.CreateSoleTrader("Test Organisation 2");
                var aatf3 = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation2);

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
                var organisation = Organisation.CreateSoleTrader("Test Organisation");

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation);
                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation);

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
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(database.WeeeContext, organisation);

                await database.WeeeContext.SaveChangesAsync();

                dataAccess.Remove<Aatf>(null);

                database.WeeeContext.ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted).Should().Be(0);
            }
        }
    }
}
