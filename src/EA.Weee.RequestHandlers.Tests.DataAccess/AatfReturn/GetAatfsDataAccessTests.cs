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
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Xunit;
    using DatabaseWrapper = Weee.Tests.Core.Model.DatabaseWrapper;
    using ModelHelper = Weee.Tests.Core.Model.ModelHelper;

    public class GetAatfsDataAccessTests
    {
        [Fact]
        public async Task GetAatfsDataAccess_ReturnsAatfsList()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);
                var dataAccess = new GetAatfsDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "UK - England");
                var aatfContact = new AatfContact("first", "last", "position", "address1", "address2", "town", "county", "postcode", country, "telephone", "email");
                var organisation = Organisation.CreatePartnership("Koalas");
                var @operator = new Operator(organisation);
                var aatf = new Aatf("KoalaBears", competentAuthority, "123456789", AatfStatus.Approved, @operator, aatfContact);

                database.WeeeContext.AatfContacts.Add(aatfContact);
                await database.WeeeContext.SaveChangesAsync();

                await genericDataAccess.Add<Aatf>(aatf);

                var aatfList = await dataAccess.GetAatfs();
                aatfList.Should().Contain(aatf);
            }
        }
    }
}
