namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using FluentAssertions;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Return = Domain.AatfReturn.Return;

    public class GetReturnObligatedCsvDataTests
    {
        private readonly EA.Weee.Domain.Organisation.Organisation organisation;
        private readonly DateTime date;
        private const string B2C = "B2C";
        private const string B2B = "B2B";
        private const string TotalReceivedHeading = "Total obligated WEEE received on behalf of PCS(s) (t)";
        private const string TotalReusedHeading = "Total obligated WEEE reused as a whole appliance (t)";
        private const string TotalSentOnHeading = "Total obligated WEEE sent to another AATF / ATF for treatment (t)";
        private const string Category = "Category";
        private const string Obligation = "Obligation";
        private const string SubmittedDate = "Submitted date (GMT)";
        private const string SubmittedBy = "Submitted by";
        private const string ApprovalNumber = "AATF approval number";
        private const string Name = "Name of AATF";
        private const string Quarter = "Quarter";
        private const string ComplianceYear = "Compliance Year";

        public GetReturnObligatedCsvDataTests()
        {
            date = new DateTime(2019, 08, 09, 11, 12, 00);
            organisation = EA.Weee.Domain.Organisation.Organisation.CreateSoleTrader("company");
        }

        [Fact]
        public async Task Execute_GivenNoAatf_NoResultsShouldBeReturned()
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

        [Fact]
        public async Task Execute_GivenAatfs_DefaultDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);

                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(28);

                AssertRow(results, aatf, db, 0, "1. Large household appliances", B2C);
                AssertRow(results, aatf, db, 1, "2. Small household appliances", B2C);
                AssertRow(results, aatf, db, 2, "3. IT and telecommunications equipment", B2C);
                AssertRow(results, aatf, db, 3, "4. Consumer equipment", B2C);
                AssertRow(results, aatf, db, 4, "5. Lighting equipment", B2C);
                AssertRow(results, aatf, db, 5, "6. Electrical and electronic tools", B2C);
                AssertRow(results, aatf, db, 6, "7. Toys, leisure and sports equipment", B2C);
                AssertRow(results, aatf, db, 7, "8. Medical devices", B2C);
                AssertRow(results, aatf, db, 8, "9. Monitoring and control instruments", B2C);
                AssertRow(results, aatf, db, 9, "10. Automatic dispensers", B2C);
                AssertRow(results, aatf, db, 10, "11. Display equipment", B2C);
                AssertRow(results, aatf, db, 11, "12. Appliances containing refrigerants", B2C);
                AssertRow(results, aatf, db, 12, "13. Gas discharge lamps and LED light sources", B2C);
                AssertRow(results, aatf, db, 13, "14. Photovoltaic panels", B2C);
                AssertRow(results, aatf, db, 14, "1. Large household appliances", B2B);
                AssertRow(results, aatf, db, 15, "2. Small household appliances", B2B);
                AssertRow(results, aatf, db, 16, "3. IT and telecommunications equipment", B2B);
                AssertRow(results, aatf, db, 17, "4. Consumer equipment", B2B);
                AssertRow(results, aatf, db, 18, "5. Lighting equipment", B2B);
                AssertRow(results, aatf, db, 19, "6. Electrical and electronic tools", B2B);
                AssertRow(results, aatf, db, 20, "7. Toys, leisure and sports equipment", B2B);
                AssertRow(results, aatf, db, 21, "8. Medical devices", B2B);
                AssertRow(results, aatf, db, 22, "9. Monitoring and control instruments", B2B);
                AssertRow(results, aatf, db, 23, "10. Automatic dispensers", B2B);
                AssertRow(results, aatf, db, 24, "11. Display equipment", B2B);
                AssertRow(results, aatf, db, 25, "12. Appliances containing refrigerants", B2B);
                AssertRow(results, aatf, db, 26, "13. Gas discharge lamps and LED light sources", B2B);
                AssertRow(results, aatf, db, 27, "14. Photovoltaic panels", B2B);
            }
        }

        private void AssertRow(DataTable results, Aatf aatf, DatabaseWrapper db, int row, string category, string obligation)
        {
            results.Rows[row][ComplianceYear].Should().Be(2019);
            results.Rows[row][Quarter].Should().Be("Q1");
            results.Rows[row][Name].Should().Be(aatf.Name);
            results.Rows[row][ApprovalNumber].Should().Be(aatf.ApprovalNumber);
            results.Rows[row][SubmittedBy].Should().Be($"{db.Model.AspNetUsers.First().FirstName} {db.Model.AspNetUsers.First().Surname}");
            results.Rows[row][SubmittedDate].Should().Be(date);
            results.Rows[row][Obligation].Should().Be(obligation);
            results.Rows[row][Category].Should().Be(category);
            results.Rows[row][TotalReceivedHeading].ToString().Should().BeEmpty();
            results.Rows[row][TotalReusedHeading].ToString().Should().BeEmpty();
            results.Rows[row][TotalSentOnHeading].ToString().Should().BeEmpty();
        }

        private Return SetupReturn(DatabaseWrapper db)
        {
            SystemTime.Freeze(date);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            return @return;
            SystemTime.Unfreeze();
        }
    }
}
