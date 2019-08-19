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
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;
    using WeeeSentOnAmount = Domain.AatfReturn.WeeeSentOnAmount;

    public class GetAllAatfSentOnDataCsvTests
    {
        private readonly EA.Weee.Domain.Organisation.Organisation organisation;

        public GetAllAatfSentOnDataCsvTests()
        {
            organisation = EA.Weee.Domain.Organisation.Organisation.CreateSoleTrader("company");
        }

        [Fact]
        public async Task Execute_GivenNoAatfsData_NoResultsShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupReturn(db);

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);

                results.Rows.Count.Should().Be(0);
            }
        }

        private Return SetupReturn(DatabaseWrapper db)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
        }
    }
}
