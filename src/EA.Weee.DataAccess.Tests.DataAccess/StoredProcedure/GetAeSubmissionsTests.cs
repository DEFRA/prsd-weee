﻿namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using Domain.AatfReturn;
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

    public class GetAeSubmissionsTests
    {
        [Fact]
        public async Task Execute_GivenAeNilReturns_DataShouldBeCorrect()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return1 = CreateSubmittedReturn(db);
                var @return2 = CreateSubmittedReturn(db);
                var @return3 = CreateSubmittedReturn(db, 2020);

                var ae = ObligatedWeeeIntegrationCommon.CreateAe(db, @return1.Organisation);

                var ae1 = ObligatedWeeeIntegrationCommon.CreateAe(db, @return1.Organisation, 2020);

                db.WeeeContext.Returns.Add(@return1);
                db.WeeeContext.Returns.Add(@return2);
                db.WeeeContext.Returns.Add(@return3);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(ae, @return1));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(ae, @return2));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(ae1, @return3));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAeSubmissions(ae.Id, ae.ComplianceYear);

                results.Count.Should().Be(2);
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeSentOnHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeSentOnNonHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeReceivedHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeReceivedNonHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeReusedHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return1.Id)).WeeeReusedNonHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return1.Id)).SubmittedBy.Should().Be(db.Model.AspNetUsers.First().FirstName + " " + db.Model.AspNetUsers.First().Surname);
                results.First(r => r.ReturnId.Equals(@return1.Id)).SubmittedDate.Date.Should().Be(@return1.SubmittedDate.Value.Date);
                results.First(r => r.ReturnId.Equals(@return1.Id)).ComplianceYear.Should().Be(@return1.Quarter.Year);
                results.First(r => r.ReturnId.Equals(@return1.Id)).Quarter.Should().Be((int)@return1.Quarter.Q);
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeSentOnHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeSentOnNonHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeReceivedHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeReceivedNonHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeReusedHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return2.Id)).WeeeReusedNonHouseHold.Should().BeNull();
                results.First(r => r.ReturnId.Equals(@return2.Id)).SubmittedBy.Should().Be(db.Model.AspNetUsers.First().FirstName + " " + db.Model.AspNetUsers.First().Surname);
                results.First(r => r.ReturnId.Equals(@return2.Id)).SubmittedDate.Date.Should().Be(@return2.SubmittedDate.Value.Date);
                results.First(r => r.ReturnId.Equals(@return2.Id)).ComplianceYear.Should().Be(@return2.Quarter.Year);
                results.First(r => r.ReturnId.Equals(@return2.Id)).Quarter.Should().Be((int)@return2.Quarter.Q);
            }
        }

        [Fact]
        public async Task Execute_GivenNonSubmittedAeNilReturns_ResultsShouldBeZero()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, db.Model.AspNetUsers.First().Id, FacilityType.Ae);
                var ae = ObligatedWeeeIntegrationCommon.CreateAe(db, @return.Organisation);

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAeSubmissions(ae.Id, ae.ComplianceYear);

                results.Count.Should().Be(0);
            }
        }

        [Fact]
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

        private static Return CreateSubmittedReturn(DatabaseWrapper db, int year = 2019)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(null, db.Model.AspNetUsers.First().Id, FacilityType.Ae, year, QuarterType.Q1);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
        }
    }
}
