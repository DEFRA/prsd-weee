namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System.Linq;
    using Charges;
    using Domain.AatfReturn;
    using RequestHandlers.AatfReturn;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Weee.Tests.Core.Model;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Operator = Domain.AatfReturn.Operator;

    public class AatfDataAccessTests
    {
        [Fact]
        public async Task GetByOrganisationId_GivenValidOrganisationId_AatfsShouldBeReturned()
        {
            using (var database = new DatabaseWrapper())
            {
                var modelHelper = new ModelHelper(database.Model);

                var dataAccess = new AatfDataAccess(database.WeeeContext);
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

                var aatfInfo = await dataAccess.GetByOrganisationId(organisation.Id);

                aatfInfo.Count.Should().Be(2);
                aatfInfo.Where(a => Equals(aatf1)).Should().NotBeNull();
                aatfInfo.Where(a => Equals(aatf2)).Should().NotBeNull();
                aatfInfo.Where(a => Equals(aatf3)).Should().BeEmpty();
            }
        }
    }
}
