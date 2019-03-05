namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using EA.Weee.RequestHandlers.Charges;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Country = Domain.Country;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeReusedSite = Domain.AatfReturn.WeeeReusedSite;

    public class GetAatfSiteDataAccessTests
    {
        private readonly DbContextHelper dbContextHelper;
        private readonly Organisation organisation;
        private readonly Operator @operator;

        public GetAatfSiteDataAccessTests()
        {
            dbContextHelper = new DbContextHelper();
            organisation = Organisation.CreatePartnership("Dummy");
            @operator = new Operator(organisation);
        }

        [Fact]
        public async Task GetAatfSiteDataAccess_ReturnedListContainsAddresses()
        {
            using (var database = new DatabaseWrapper())
            {
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);
                var dataAccess = new GetAatfSiteDataAccess(database.WeeeContext, genericDataAccess);
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);

                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var aatf = CreateAatf(competentAuthority, @operator);
                var @return = new Return(@operator, new Quarter(2019, QuarterType.Q1), ReturnStatus.Created);
                
                await genericDataAccess.Add<Organisation>(organisation);
                await genericDataAccess.Add<Aatf>(aatf);
                await genericDataAccess.Add<Return>(@return);

                var weeeReused = new WeeeReused(aatf.Id, @return.Id);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var aatfAddress = CreateAatfAddress(country);
                var weeeReusedSite = new WeeeReusedSite(weeeReused, aatfAddress);

                await genericDataAccess.Add<WeeeReused>(weeeReused);
                await genericDataAccess.Add<AatfAddress>(aatfAddress);
                await genericDataAccess.Add<WeeeReusedSite>(weeeReusedSite);
                
                List<AatfAddress> addressList = await dataAccess.GetAddresses(aatf.Id, @return.Id);

                addressList.Should().Contain(aatfAddress);
            }
        }

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority, Operator @operator)
        {
            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                @operator);
        }

        private AatfAddress CreateAatfAddress(Country country)
        {
            return new AatfAddress("Site Name",
                "Address 1",
                "Address 2",
                "Town Or City",
                "County Or Region",
                "Postcode",
                country);
        }
    }
}
