namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Assert = Xunit.Assert;
    using Return = Domain.AatfReturn.Return;
    using WeeeReusedSite = Domain.AatfReturn.WeeeReusedSite;

    public class GetAllAatfReuseSitesCsvDataTests
    {
        [Fact]
        public async Task Execute_GivenNoData_NoResultsShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
                @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAllAatfReuseSitesCsvData(2019, string.Empty, null, null);

                results.Count.Should().Be(0);
            }
        }

        private static Return CreateSubmittedReturn(DatabaseWrapper db)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
        }
    }
}
