namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Admin.Aatf;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Charges;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;
    using DatabaseWrapper = Weee.Tests.Core.Model.DatabaseWrapper;
    using ModelHelper = Weee.Tests.Core.Model.ModelHelper;
    public class GetAatfsDataAccessTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfsDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly Organisation organisation;
        private readonly Operator @operator;
        public GetAatfsDataAccessTests()
        {
            authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            organisation = Organisation.CreatePartnership("Koalas");
            @operator = new Operator(organisation);
        }

        [Fact]
        public async Task GetAatfsDataAccess_ReturnsAatfsList()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);
                var dataAccess = new GetAatfsDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);
                var competantAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var competantAuthority = await competantAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var aatf = new Aatf("KoalaBears", competantAuthority, "123456789", AatfStatus.Approved, @operator);

                await genericDataAccess.Add<Aatf>(aatf);

                List<Aatf> aatfList = await dataAccess.GetAatfs();
                aatfList.Should().Contain(aatf);
            }
        }
    }
}
