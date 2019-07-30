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

                var results = await db.StoredProcedures.GetAllAatfSentOnDataCsv(2019, string.Empty, string.Empty, null, null);

                results.Tables[0].Rows.Count.Equals(0);
                results.Tables[1].Rows.Count.Equals(0);
            }
        }

        [Fact]
        public async Task Execute_GivenWeeeSentOnData_ReturnsWeeeSentOnAatfDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);

                await CreateWeeSentOnData(db, aatf, @return);

                var results = await db.StoredProcedures.GetAllAatfSentOnDataCsv(2019, string.Empty, string.Empty, null, null);

                Assert.NotNull(results);
                var data = from x in results.Tables[0].AsEnumerable()
                           where x.Field<string>("Name of AATF") == aatf.Name
                           select x;
                data.AsQueryable().Count().Should().Be(28);

                var dataB2B = from x in results.Tables[0].AsEnumerable()
                              where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Obligation") == "B2B"
                              select x;
                dataB2B.AsQueryable().Count().Should().Be(14);

                var dataB2C = from x in results.Tables[0].AsEnumerable()
                              where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Obligation") == "B2C"
                              select x;
                dataB2C.AsQueryable().Count().Should().Be(14);
            }
        }

        [Theory]
        [InlineData("B2B", "B2C")]
        [InlineData("B2C", "B2B")]
        public async Task Execute_GivenWeeeSentOnData_ObligationTypeParameter_ReturnsDataShouldBeCorrect(string filter, string nonFilter)
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);

                await CreateWeeSentOnData(db, aatf, @return);

                var results = await db.StoredProcedures.GetAllAatfSentOnDataCsv(2019, string.Empty, filter, null, null);

                Assert.NotNull(results);

                var dataB2B = from x in results.Tables[0].AsEnumerable()
                              where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Obligation") == filter
                              select x;
                dataB2B.AsQueryable().Count().Should().Be(14);

                var dataB2C = from x in results.Tables[0].AsEnumerable()
                              where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Obligation") == nonFilter
                              select x;
                dataB2C.AsQueryable().Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task Execute_GivenWeeeSentOnData_AuthorityParameter_ReturnsDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);

                await CreateWeeSentOnData(db, aatf, @return);

                var filter = db.WeeeContext.UKCompetentAuthorities.First();

                var results = await db.StoredProcedures.GetAllAatfSentOnDataCsv(2019, string.Empty, string.Empty, filter.Id, null);

                Assert.NotNull(results);

                var data = from x in results.Tables[0].AsEnumerable()
                              where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("Appropriate authority") == filter.Abbreviation
                              select x;
                data.AsQueryable().Count().Should().Be(28);
            }
        }

        [Fact]
        public async Task Execute_GivenWeeeSentOnData_PanAreaParameter_ReturnsDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);

                await CreateWeeSentOnData(db, aatf, @return);

                var ea = db.WeeeContext.UKCompetentAuthorities.First();

                var filter = db.WeeeContext.PanAreas.First();

                var results = await db.StoredProcedures.GetAllAatfSentOnDataCsv(2019, string.Empty, string.Empty, ea.Id, filter.Id);

                Assert.NotNull(results);

                var data = from x in results.Tables[0].AsEnumerable()
                           where x.Field<string>("Name of AATF") == aatf.Name && x.Field<string>("WROS pan area team") == filter.Name
                           select x;
                data.AsQueryable().Count().Should().Be(28);
            }
        }

        private static Return CreateSubmittedReturn(DatabaseWrapper db)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
        }

        private static async Task CreateWeeSentOnData(DatabaseWrapper db, EA.Weee.Domain.AatfReturn.Aatf aatf, Return @return)
        {
            var weeeSentOn = new EA.Weee.Domain.AatfReturn.WeeeSentOn(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return);
            var weeeSentOnAmounts = new List<WeeeSentOnAmount>()
                {
                    new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOn, 1, 1, 2),
                    new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOn, 2, 3, 4)
                };

            db.WeeeContext.Returns.Add(@return);
            db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
            db.WeeeContext.WeeeSentOn.Add(weeeSentOn);
            db.WeeeContext.WeeeSentOnAmount.AddRange(weeeSentOnAmounts);

            await db.WeeeContext.SaveChangesAsync();

            db.WeeeContext.ReturnReportOns.Add(new ReturnReportOn(@return.Id, 2));

            await db.WeeeContext.SaveChangesAsync();
        }
    }
}
