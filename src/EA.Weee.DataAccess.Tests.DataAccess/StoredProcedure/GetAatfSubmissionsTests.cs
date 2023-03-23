namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using Domain.AatfReturn;
    using Domain.Admin.AatfReports;
    using EA.Weee.Domain.DataReturns;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Return = Domain.AatfReturn.Return;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;
    using WeeeReusedAmount = Domain.AatfReturn.WeeeReusedAmount;
    using WeeeSentOnAmount = Domain.AatfReturn.WeeeSentOnAmount;

    public class GetAatfSubmissionsTests
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

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(1);
                results.ElementAt(0).WeeeReceivedHouseHold.Should().Be(4);
                results.ElementAt(0).WeeeReceivedNonHouseHold.Should().Be(6);
                AssertFixedPropertiesForReturn(results, db, @return);
            }
        }

        public async Task Execute_GivenNonSubmittedReturn_ReturnShouldNotBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(@return.Organisation);
                var weeeReceived = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf, @return);

                var weeeReceivedAmounts = new List<WeeeReceivedAmount>()
                {
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 1, 1, 2),
                };

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.WeeeReceived.Add(weeeReceived);
                db.WeeeContext.WeeeReceivedAmount.AddRange(weeeReceivedAmounts);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(0);
            }
        }

        [Fact]
        public async Task Execute_GivenNullWeeeReceivedData_ReturnsWeeeReceivedAatfDataShouldBeNull()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);
                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(@return.Organisation);
                var weeeReceived = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf, @return);

                var weeeReceivedAmounts = new List<WeeeReceivedAmount>()
                {
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 1, null, null),
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 2, null, null)
                };

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.WeeeReceived.Add(weeeReceived);
                db.WeeeContext.WeeeReceivedAmount.AddRange(weeeReceivedAmounts);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(1);
                results.ElementAt(0).WeeeReceivedHouseHold.Should().BeNull();
                results.ElementAt(0).WeeeReceivedNonHouseHold.Should().BeNull();
                AssertFixedPropertiesForReturn(results, db, @return);
            }
        }

        [Fact]
        public async Task Execute_GivenWeeeReusedData_ReturnsWeeeReusedAatfDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);
                var weeeReused = new EA.Weee.Domain.AatfReturn.WeeeReused(aatf, @return);

                var weeeReusedAmounts = new List<WeeeReusedAmount>()
                {
                    new Domain.AatfReturn.WeeeReusedAmount(weeeReused, 1, 1, 2),
                    new Domain.AatfReturn.WeeeReusedAmount(weeeReused, 2, 3, 4)
                };

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.WeeeReused.Add(weeeReused);
                db.WeeeContext.WeeeReusedAmount.AddRange(weeeReusedAmounts);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(1);
                results.ElementAt(0).WeeeReusedHouseHold.Should().Be(4);
                results.ElementAt(0).WeeeReusedNonHouseHold.Should().Be(6);
                AssertFixedPropertiesForReturn(results, db, @return);
            }
        }

        [Fact]
        public async Task Execute_GivenNullWeeeReusedData_ReturnsWeeeReusedAatfDataShouldBeNull()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);
                var weeeReused = new EA.Weee.Domain.AatfReturn.WeeeReused(aatf, @return);

                var weeeReusedAmounts = new List<WeeeReusedAmount>()
                {
                    new Domain.AatfReturn.WeeeReusedAmount(weeeReused, 1, null, null),
                    new Domain.AatfReturn.WeeeReusedAmount(weeeReused, 2, null, null)
                };

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.WeeeReused.Add(weeeReused);
                db.WeeeContext.WeeeReusedAmount.AddRange(weeeReusedAmounts);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(1);
                results.ElementAt(0).WeeeReusedHouseHold.Should().BeNull();
                results.ElementAt(0).WeeeReusedNonHouseHold.Should().BeNull();
                AssertFixedPropertiesForReturn(results, db, @return);
            }
        }

        [Fact]
        public async Task Execute_GivenWeeeSentOnData_ReturnsWeeeSentOnAatfDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);
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

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(1);
                results.ElementAt(0).WeeeSentOnHouseHold.Should().Be(4);
                results.ElementAt(0).WeeeSentOnNonHouseHold.Should().Be(6);
                AssertFixedPropertiesForReturn(results, db, @return);
            }
        }

        [Fact]
        public async Task Execute_GivenNullWeeeSentOnData_ReturnsWeeeSentOnAatfDataShouldBeNull()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = CreateSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation);
                var weeeSentOn = new EA.Weee.Domain.AatfReturn.WeeeSentOn(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return);

                var weeeSentOnAmounts = new List<WeeeSentOnAmount>()
                {
                    new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOn, 1, null, null),
                    new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOn, 2, null, null)
                };

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.WeeeSentOn.Add(weeeSentOn);
                db.WeeeContext.WeeeSentOnAmount.AddRange(weeeSentOnAmounts);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(1);
                results.ElementAt(0).WeeeSentOnHouseHold.Should().BeNull();
                results.ElementAt(0).WeeeSentOnNonHouseHold.Should().BeNull();
                AssertFixedPropertiesForReturn(results, db, @return);
            }
        }

        [Fact]
        public async Task Execute_GivenMultipleReturns_ReturnDataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return1 = CreateSubmittedReturn(db);
                var @return2 = CreateSubmittedReturn(db);

                var scheme = ObligatedWeeeIntegrationCommon.CreateScheme(@return1.Organisation);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return1.Organisation);

                var weeeSentOn1 = new EA.Weee.Domain.AatfReturn.WeeeSentOn(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return1);
                var weeeSentOnAmount1 = new EA.Weee.Domain.AatfReturn.WeeeSentOnAmount(weeeSentOn1, 1, 1, 2);

                var weeeReceived1 = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf, @return1);
                var weeeReceivedAmount1 = new EA.Weee.Domain.AatfReturn.WeeeReceivedAmount(weeeReceived1, 1, 3, 4);

                var weeeReused1 = new EA.Weee.Domain.AatfReturn.WeeeReused(aatf, @return1);
                var weeeReusedAmount1 = new Domain.AatfReturn.WeeeReusedAmount(weeeReused1, 1, 5, 6);

                var weeeSentOn2 = new EA.Weee.Domain.AatfReturn.WeeeSentOn(ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return2);
                var weeeSentOnAmount2 = new EA.Weee.Domain.AatfReturn.WeeeSentOnAmount(weeeSentOn2, 1, 7, 8);

                var weeeReceived2 = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf, @return2);
                var weeeReceivedAmount2 = new EA.Weee.Domain.AatfReturn.WeeeReceivedAmount(weeeReceived2, 1, 9, 10);

                var weeeReused2 = new EA.Weee.Domain.AatfReturn.WeeeReused(aatf, @return2);
                var weeeReusedAmount2 = new Domain.AatfReturn.WeeeReusedAmount(weeeReused2, 1, 11, 12);

                db.WeeeContext.Returns.Add(@return1);
                db.WeeeContext.Returns.Add(@return2);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return1));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return2));
                db.WeeeContext.WeeeSentOn.Add(weeeSentOn1);
                db.WeeeContext.WeeeSentOn.Add(weeeSentOn2);
                db.WeeeContext.WeeeSentOnAmount.Add(weeeSentOnAmount1);
                db.WeeeContext.WeeeSentOnAmount.Add(weeeSentOnAmount2);
                db.WeeeContext.WeeeReceived.Add(weeeReceived1);
                db.WeeeContext.WeeeReceived.Add(weeeReceived2);
                db.WeeeContext.WeeeReceivedAmount.Add(weeeReceivedAmount1);
                db.WeeeContext.WeeeReceivedAmount.Add(weeeReceivedAmount2);
                db.WeeeContext.WeeeReused.Add(weeeReused1);
                db.WeeeContext.WeeeReused.Add(weeeReused2);
                db.WeeeContext.WeeeReusedAmount.Add(weeeReusedAmount1);
                db.WeeeContext.WeeeReusedAmount.Add(weeeReusedAmount2);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(2);
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeSentOnHouseHold.Should().Be(1);
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeSentOnNonHouseHold.Should().Be(2);
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeReceivedHouseHold.Should().Be(3);
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeReceivedNonHouseHold.Should().Be(4);
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeReusedHouseHold.Should().Be(5);
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeReusedNonHouseHold.Should().Be(6);
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeSentOnHouseHold.Should().Be(7);
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeSentOnNonHouseHold.Should().Be(8);
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeReceivedHouseHold.Should().Be(9);
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeReceivedNonHouseHold.Should().Be(10);
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeReusedHouseHold.Should().Be(11);
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeReusedNonHouseHold.Should().Be(12);
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

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(0);
            }
        }

        [Fact]
        public async Task Execute_GivenAatfCY_ReturnsWeeeReceivedAatfForCYDataShouldBeCorrect()
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

                var aatf1 = ObligatedWeeeIntegrationCommon.CreateAatf(db, @return.Organisation, 2020);
                var @return2020 = CreateSubmittedReturn(db, 2020);
                var weeeReceived2020 = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme, aatf1, @return2020);

                var weeeReceivedAmounts2020 = new List<WeeeReceivedAmount>()
                {
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived2020, 1, 1, 2),
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived2020, 2, 3, 4),
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived2020, 3, 3, 4)
                };

                db.WeeeContext.Returns.Add(@return2020);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf1, @return2020));
                db.WeeeContext.WeeeReceived.Add(weeeReceived2020);
                db.WeeeContext.WeeeReceivedAmount.AddRange(weeeReceivedAmounts2020);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfSubmissions(aatf.Id, aatf.ComplianceYear);

                results.Count.Should().Be(1);
                results.ElementAt(0).WeeeReceivedHouseHold.Should().Be(4);
                results.ElementAt(0).WeeeReceivedNonHouseHold.Should().Be(6);
                AssertFixedPropertiesForReturn(results, db, @return);
            }
        }

        private void AssertFixedPropertiesForReturn(List<AatfSubmissionHistory> results, DatabaseWrapper db, Return @return)
        {
            results.ElementAt(0).SubmittedBy.Should().Be(db.Model.AspNetUsers.First().FirstName + " " + db.Model.AspNetUsers.First().Surname);
            results.ElementAt(0).SubmittedDate.Date.Should().Be(@return.SubmittedDate.Value.Date);
            results.ElementAt(0).ComplianceYear.Should().Be(@return.Quarter.Year);
            results.ElementAt(0).Quarter.Should().Be((int)@return.Quarter.Q);
        }

        private static Return CreateSubmittedReturn(DatabaseWrapper db, int year = 2019)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, year, QuarterType.Q1);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
        }
    }
}
