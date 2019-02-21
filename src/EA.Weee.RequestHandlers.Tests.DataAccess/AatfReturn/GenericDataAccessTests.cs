namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Charges;
    using Domain;
    using Domain.AatfReturn;
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

                var aatf = CreateAatf(competantAuthority);

                var result = await dataAccess.Add<Aatf>(aatf);

                result.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task GetManyByExpression_GivenGetManyByExpressionSpecification_AatfsShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var competantAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);

                var organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                var @operator = new Operator(organisation);
                var competantAuthority = await competantAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var aatf1 = new Aatf("Name1", competantAuthority, "approval1", AatfStatus.Approved, @operator);
                var aatf2 = new Aatf("Name2", competantAuthority, "approval2", AatfStatus.Approved, @operator);

                var organisation2 = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation 2");
                var @operator2 = new Operator(organisation);
                var aatf3 = new Aatf("Name3", competantAuthority, "approval1", AatfStatus.Approved, @operator2);

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

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority)
        {
            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                new Operator(Organisation.CreatePartnership("trading")));
        }
    }
}
