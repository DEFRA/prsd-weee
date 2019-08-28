namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using Domain.AatfReturn;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Assert = Xunit.Assert;
    using Return = Domain.AatfReturn.Return;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

    public class GetAatfObligatedCsvDataTests
    {
        [Fact]
        public async Task Execute_GivenWeeeReceivedData_ReturnsWeeeReceivedAatfDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(@return.Organisation);
                scheme.UpdateScheme("TestScheme", "WEE/AB1234CD/SCH", string.Empty, null, aatf.CompetentAuthority);

                var weeeReceived = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf, @return);

                var weeeReceivedAmounts = new List<WeeeReceivedAmount>()
                {
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 1, 1, 2),
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 2, 3, 4)
                };

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.WeeeReceived.Add(weeeReceived);
                db.WeeeContext.WeeeReceivedAmount.AddRange(weeeReceivedAmounts);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfObligatedCsvData(@return.Id, 2019, 1, aatf.Id);

                Assert.NotNull(results);
                var data = from x in results.AsEnumerable()
                           where x.Field<string>("Name of AATF") == aatf.Name
                           select x;
                data.AsQueryable().Count().Should().Be(28);

                var dataB2B = from x in results.AsEnumerable()
                              where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Obligation type") == "B2B"
                              select x;
                dataB2B.AsQueryable().Count().Should().Be(14);

                var dataB2C = from x in results.AsEnumerable()
                              where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Obligation type") == "B2C"
                              select x;
                dataB2C.AsQueryable().Count().Should().Be(14);
            }
        }

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

                var results = await db.StoredProcedures.GetAatfObligatedCsvData(@return.Id, 2019, 1, aatf.Id);

                results.Rows.Count.Equals(0);
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
