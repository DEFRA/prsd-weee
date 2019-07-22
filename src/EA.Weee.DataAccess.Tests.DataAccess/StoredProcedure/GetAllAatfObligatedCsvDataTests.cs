namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Return = Domain.AatfReturn.Return;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

    public class GetAllAatfObligatedCsvDataTests
    {
        [Fact]
        public async Task Execute_GivenWeeeReceivedData_ReturnsWeeeReceivedAatfDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(@return.Organisation);
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

                var results = await db.StoredProcedures.GetAllAatfObligatedCsvData(2019, string.Empty, string.Empty, null, null, 1); 

                Assert.NotNull(results);
               var data = from x in results.AsEnumerable() 
                          where x.Field<string>("Name of AATF") == aatf.Name
                          select x;
                data.AsEnumerable().Count().Equals(4);

                var dataB2B = from x in results.AsEnumerable()
                           where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Obligation") == "B2B"
                           select x;
                dataB2B.AsEnumerable().Count().Equals(2);

                var dataB2C = from x in results.AsEnumerable()
                              where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Obligation") == "B2C"
                              select x;
                dataB2C.AsEnumerable().Count().Equals(2);
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

                var results = await db.StoredProcedures.GetAllAatfObligatedCsvData(2019, string.Empty, string.Empty, null, null, 1);

                results.AsEnumerable().Count().Equals(0);
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
