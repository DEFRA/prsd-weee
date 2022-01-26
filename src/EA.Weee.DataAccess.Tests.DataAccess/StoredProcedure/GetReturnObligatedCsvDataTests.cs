namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using AutoFixture;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Domain.Obligation;
    using FluentAssertions;
    using Prsd.Core;
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using FacilityType = Domain.AatfReturn.FacilityType;
    using Return = Domain.AatfReturn.Return;
    using ReturnScheme = Domain.AatfReturn.ReturnScheme;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceived = Domain.AatfReturn.WeeeReceived;
    using WeeeReused = Domain.AatfReturn.WeeeReused;
    using WeeeSentOn = Domain.AatfReturn.WeeeSentOn;

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
        private const string Obligation = "Obligation Type";
        private const string SubmittedDate = "Submitted date (GMT)";
        private const string SubmittedBy = "Submitted by";
        private const string ApprovalNumber = "AATF approval number";
        private const string Name = "Name of AATF";
        private const string Quarter = "Quarter";
        private const string ComplianceYear = "Compliance Year";
        private readonly Fixture fixture;

        public GetReturnObligatedCsvDataTests()
        {
            fixture = new Fixture();

            date = new DateTime(2019, 08, 09, 11, 12, 00);
            organisation = EA.Weee.Domain.Organisation.Organisation.CreateSoleTrader("company");
        }

        [Fact]
        public async Task Execute_GivenNoAatf_NoResultsShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupSubmittedReturn(db);

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);

                results.Rows.Count.Should().Be(0);
                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenSubmittedReturnWithWithoutOptionsSelected_DataShouldNotHaveColumns()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var aatf = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAA");

                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(28);

                results.Columns[TotalReceivedHeading].Should().BeNull();
                results.Columns[TotalSentOnHeading].Should().BeNull();
                results.Columns[TotalReusedHeading].Should().BeNull();
                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenSubmittedReturnWithWithOptionsSelected_DataShouldHaveColumns()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var aatf = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAA");

                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 2));
                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 3));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(28);

                results.Columns[TotalReceivedHeading].Should().NotBeNull();
                results.Columns[TotalSentOnHeading].Should().NotBeNull();
                results.Columns[TotalReusedHeading].Should().NotBeNull();
                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithNoDataAndSubmittedReturn_DefaultDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);

                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 2));
                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 3));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(28);

                for (var countValue = 0; countValue < CategoryValues().Count(); countValue++)
                {
                    var value = CategoryValues().ElementAt(countValue);
                    var categoryNumber = (value.CategoryId <= 9) ? "0" + Convert.ToString(value.CategoryId) : Convert.ToString(value.CategoryId);
                    AssertSubmittedRow(results, aatf, db, countValue, $"{categoryNumber}. {value.CategoryDisplay}", B2C);
                }

                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfWithNoDataAndCreatedReturn_DefaultDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var aatf = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAA");

                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));
                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 2));
                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 3));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(28);

                for (var countValue = 0; countValue < CategoryValues().Count(); countValue++)
                {
                    var value = CategoryValues().ElementAt(countValue);
                    var categoryNumber = (value.CategoryId <= 9) ? "0" + Convert.ToString(value.CategoryId) : Convert.ToString(value.CategoryId);
                    AssertCreatedRow(results, aatf, countValue, $"{categoryNumber}. {value.CategoryDisplay}", B2C);
                }

                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfAndAatfThatShouldNotReport_AatfShouldNotBeInData()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var aatf = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAA");
                var aatfNotReporting = SetupAatfWithApprovalDate(db, DateTime.MaxValue, "AAA");

                db.WeeeContext.Aatfs.Add(aatf);
                db.WeeeContext.Aatfs.Add(aatfNotReporting);
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(28);

                results.Select($"AatfKey='{aatf.Id}'").Length.Should().Be(28);
                results.Select($"AatfKey='{aatfNotReporting.Id}'").Length.Should().Be(0);

                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfsWithReceivedDataAndSubmittedReturn_DataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);
                aatf.UpdateDetails("AAA", aatf.CompetentAuthority, aatf.ApprovalNumber, aatf.AatfStatus, aatf.Organisation, aatf.Size, aatf.ApprovalDate, aatf.LocalArea, aatf.PanArea);

                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);
                aatf2.UpdateDetails("AAB", aatf.CompetentAuthority, aatf.ApprovalNumber, aatf.AatfStatus, aatf.Organisation, aatf.Size, aatf.ApprovalDate, aatf.LocalArea, aatf.PanArea);

                var scheme1 = new Scheme(organisation);
                scheme1.UpdateScheme("scheme1", "1111", "1111", fixture.Create<ObligationType>(), db.WeeeContext.UKCompetentAuthorities.First());

                var scheme2 = new Scheme(organisation);
                scheme2.UpdateScheme("scheme2", "1111", "1111", fixture.Create<ObligationType>(), db.WeeeContext.UKCompetentAuthorities.First());

                var weeeReceivedAatf1Scheme1 = new WeeeReceived(scheme1, aatf, @return);
                var weeeReceivedAatf1Scheme2 = new WeeeReceived(scheme2, aatf, @return);
                var weeeReceivedAatf2Scheme2 = new WeeeReceived(scheme2, aatf2, @return);

                foreach (var categoryValue in CategoryValues())
                {
                    db.WeeeContext.WeeeReceivedAmount.Add(new Domain.AatfReturn.WeeeReceivedAmount(weeeReceivedAatf1Scheme1, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeReceivedAmount.Add(new Domain.AatfReturn.WeeeReceivedAmount(weeeReceivedAatf1Scheme2, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeReceivedAmount.Add(new Domain.AatfReturn.WeeeReceivedAmount(weeeReceivedAatf2Scheme2, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                }

                db.WeeeContext.ReturnScheme.Add(new ReturnScheme(scheme1, @return));
                db.WeeeContext.ReturnScheme.Add(new ReturnScheme(scheme2, @return));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf2, @return));
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(56);

                foreach (var categoryValue in CategoryValues())
                {
                    var houseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf1[0][TotalReceivedHeading].Should().Be(categoryValue.CategoryId * 2); // two schemes
                    houseHoldResultAatf1[0]["Obligated WEEE received on behalf of scheme1 (t)"].Should().Be(categoryValue.CategoryId);
                    houseHoldResultAatf1[0]["Obligated WEEE received on behalf of scheme2 (t)"].Should().Be(categoryValue.CategoryId);

                    nonHouseHoldResultAatf1[0][TotalReceivedHeading].Should().Be((categoryValue.CategoryId + 1) * 2); // two schemes
                    nonHouseHoldResultAatf1[0]["Obligated WEEE received on behalf of scheme1 (t)"].Should().Be((categoryValue.CategoryId + 1));
                    nonHouseHoldResultAatf1[0]["Obligated WEEE received on behalf of scheme2 (t)"].Should().Be(categoryValue.CategoryId + 1);

                    var houseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf2[0][TotalReceivedHeading].Should().Be(categoryValue.CategoryId); //  single scheme
                    houseHoldResultAatf2[0]["Obligated WEEE received on behalf of scheme2 (t)"].Should().Be(categoryValue.CategoryId);
                    nonHouseHoldResultAatf2[0][TotalReceivedHeading].Should().Be((categoryValue.CategoryId + 1)); // single schemes
                    nonHouseHoldResultAatf2[0]["Obligated WEEE received on behalf of scheme2 (t)"].Should().Be(categoryValue.CategoryId + 1);
                }

                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfsWithReceivedDataAndCreatedReturn_DataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var aatf = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAA");
                var aatf2 = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAB");

                var scheme1 = new Scheme(organisation);
                scheme1.UpdateScheme("scheme1", "1111", "1111", fixture.Create<ObligationType>(), db.WeeeContext.UKCompetentAuthorities.First());

                var scheme2 = new Scheme(organisation);
                scheme2.UpdateScheme("scheme2", "1111", "1111", fixture.Create<ObligationType>(), db.WeeeContext.UKCompetentAuthorities.First());

                var weeeReceivedAatf1Scheme1 = new WeeeReceived(scheme1, aatf, @return);
                var weeeReceivedAatf1Scheme2 = new WeeeReceived(scheme2, aatf, @return);
                var weeeReceivedAatf2Scheme2 = new WeeeReceived(scheme2, aatf2, @return);

                foreach (var categoryValue in CategoryValues())
                {
                    db.WeeeContext.WeeeReceivedAmount.Add(new Domain.AatfReturn.WeeeReceivedAmount(weeeReceivedAatf1Scheme1, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeReceivedAmount.Add(new Domain.AatfReturn.WeeeReceivedAmount(weeeReceivedAatf1Scheme2, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeReceivedAmount.Add(new Domain.AatfReturn.WeeeReceivedAmount(weeeReceivedAatf2Scheme2, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                }

                db.WeeeContext.ReturnScheme.Add(new ReturnScheme(scheme1, @return));
                db.WeeeContext.ReturnScheme.Add(new ReturnScheme(scheme2, @return));

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 1));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(56);

                foreach (var categoryValue in CategoryValues())
                {
                    var houseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf1[0][TotalReceivedHeading].Should().Be(categoryValue.CategoryId * 2); // two schemes
                    houseHoldResultAatf1[0]["Obligated WEEE received on behalf of scheme1 (t)"].Should().Be(categoryValue.CategoryId);
                    houseHoldResultAatf1[0]["Obligated WEEE received on behalf of scheme2 (t)"].Should().Be(categoryValue.CategoryId);

                    nonHouseHoldResultAatf1[0][TotalReceivedHeading].Should().Be((categoryValue.CategoryId + 1) * 2); // two schemes
                    nonHouseHoldResultAatf1[0]["Obligated WEEE received on behalf of scheme1 (t)"].Should().Be((categoryValue.CategoryId + 1));
                    nonHouseHoldResultAatf1[0]["Obligated WEEE received on behalf of scheme2 (t)"].Should().Be(categoryValue.CategoryId + 1);

                    var houseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf2[0][TotalReceivedHeading].Should().Be(categoryValue.CategoryId); //  single scheme
                    houseHoldResultAatf2[0]["Obligated WEEE received on behalf of scheme2 (t)"].Should().Be(categoryValue.CategoryId);
                    nonHouseHoldResultAatf2[0][TotalReceivedHeading].Should().Be((categoryValue.CategoryId + 1)); // single schemes
                    nonHouseHoldResultAatf2[0]["Obligated WEEE received on behalf of scheme2 (t)"].Should().Be(categoryValue.CategoryId + 1);
                }

                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfsWithSentOnDataAndSubmittedReturn_DataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);
                aatf.UpdateDetails("AAA", aatf.CompetentAuthority, aatf.ApprovalNumber, aatf.AatfStatus, aatf.Organisation, aatf.Size, aatf.ApprovalDate, aatf.LocalArea, aatf.PanArea);

                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);
                aatf2.UpdateDetails("AAB", aatf.CompetentAuthority, aatf.ApprovalNumber, aatf.AatfStatus, aatf.Organisation, aatf.Size, aatf.ApprovalDate, aatf.LocalArea, aatf.PanArea);

                var siteAddressAatf1Address1 = ObligatedWeeeIntegrationCommon.CreateAatfAddress(db);
                var siteAddressAatf1Address2 = ObligatedWeeeIntegrationCommon.CreateAatfAddress(db);
                var siteAddressAatf2Address1 = ObligatedWeeeIntegrationCommon.CreateAatfAddress(db);

                var weeeSentOnAatf1SentOn1 =
                    new WeeeSentOn(siteAddressAatf1Address1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return);
                var weeeSentOnAatf1SentOn2 =
                    new WeeeSentOn(siteAddressAatf1Address2, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return);
                var weeeSentOnAatf2SentOn1 =
                    new WeeeSentOn(siteAddressAatf2Address1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf2, @return);

                foreach (var categoryValue in CategoryValues())
                {
                    db.WeeeContext.WeeeSentOnAmount.Add(new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOnAatf1SentOn1, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeSentOnAmount.Add(new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOnAatf1SentOn2, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeSentOnAmount.Add(new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOnAatf2SentOn1, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                }

                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf2, @return));
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 2));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(56);

                foreach (var categoryValue in CategoryValues())
                {
                    var houseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf1[0][TotalSentOnHeading].Should().Be(categoryValue.CategoryId * 2);
                    houseHoldResultAatf1[0][$"Obligated WEEE sent to {siteAddressAatf1Address1.Name} (t)"].Should().Be(categoryValue.CategoryId);
                    houseHoldResultAatf1[0][$"Obligated WEEE sent to {siteAddressAatf1Address2.Name} (t)"].Should().Be(categoryValue.CategoryId);

                    nonHouseHoldResultAatf1[0][TotalSentOnHeading].Should().Be((categoryValue.CategoryId + 1) * 2);
                    nonHouseHoldResultAatf1[0][$"Obligated WEEE sent to {siteAddressAatf1Address1.Name} (t)"].Should().Be((categoryValue.CategoryId + 1));
                    nonHouseHoldResultAatf1[0][$"Obligated WEEE sent to {siteAddressAatf1Address2.Name} (t)"].Should().Be(categoryValue.CategoryId + 1);

                    var houseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf2[0][TotalSentOnHeading].Should().Be(categoryValue.CategoryId);
                    houseHoldResultAatf2[0][$"Obligated WEEE sent to {siteAddressAatf2Address1.Name} (t)"].Should().Be(categoryValue.CategoryId);
                    nonHouseHoldResultAatf2[0][TotalSentOnHeading].Should().Be((categoryValue.CategoryId + 1));
                    nonHouseHoldResultAatf2[0][$"Obligated WEEE sent to {siteAddressAatf2Address1.Name} (t)"].Should().Be(categoryValue.CategoryId + 1);
                }

                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfsWithSentOnDataAndCreatedReturn_DataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var aatf = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAA");
                var aatf2 = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAB");

                var siteAddressAatf1Address1 = ObligatedWeeeIntegrationCommon.CreateAatfAddress(db);
                var siteAddressAatf1Address2 = ObligatedWeeeIntegrationCommon.CreateAatfAddress(db);
                var siteAddressAatf2Address1 = ObligatedWeeeIntegrationCommon.CreateAatfAddress(db);

                var weeeSentOnAatf1SentOn1 =
                    new WeeeSentOn(siteAddressAatf1Address1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return);
                var weeeSentOnAatf1SentOn2 =
                    new WeeeSentOn(siteAddressAatf1Address2, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf, @return);
                var weeeSentOnAatf2SentOn1 =
                    new WeeeSentOn(siteAddressAatf2Address1, ObligatedWeeeIntegrationCommon.CreateAatfAddress(db), aatf2, @return);

                foreach (var categoryValue in CategoryValues())
                {
                    db.WeeeContext.WeeeSentOnAmount.Add(new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOnAatf1SentOn1, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeSentOnAmount.Add(new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOnAatf1SentOn2, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeSentOnAmount.Add(new Domain.AatfReturn.WeeeSentOnAmount(weeeSentOnAatf2SentOn1, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                }

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 2));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(56);

                foreach (var categoryValue in CategoryValues())
                {
                    var houseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf1[0][TotalSentOnHeading].Should().Be(categoryValue.CategoryId * 2);
                    houseHoldResultAatf1[0][$"Obligated WEEE sent to {siteAddressAatf1Address1.Name} (t)"].Should().Be(categoryValue.CategoryId);
                    houseHoldResultAatf1[0][$"Obligated WEEE sent to {siteAddressAatf1Address2.Name} (t)"].Should().Be(categoryValue.CategoryId);

                    nonHouseHoldResultAatf1[0][TotalSentOnHeading].Should().Be((categoryValue.CategoryId + 1) * 2);
                    nonHouseHoldResultAatf1[0][$"Obligated WEEE sent to {siteAddressAatf1Address1.Name} (t)"].Should().Be((categoryValue.CategoryId + 1));
                    nonHouseHoldResultAatf1[0][$"Obligated WEEE sent to {siteAddressAatf1Address2.Name} (t)"].Should().Be(categoryValue.CategoryId + 1);

                    var houseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf2[0][TotalSentOnHeading].Should().Be(categoryValue.CategoryId);
                    houseHoldResultAatf2[0][$"Obligated WEEE sent to {siteAddressAatf2Address1.Name} (t)"].Should().Be(categoryValue.CategoryId);
                    nonHouseHoldResultAatf2[0][TotalSentOnHeading].Should().Be((categoryValue.CategoryId + 1));
                    nonHouseHoldResultAatf2[0][$"Obligated WEEE sent to {siteAddressAatf2Address1.Name} (t)"].Should().Be(categoryValue.CategoryId + 1);
                }

                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfsWithReusedAndSubmittedReturn_DataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupSubmittedReturn(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);
                aatf.UpdateDetails("AAA", aatf.CompetentAuthority, aatf.ApprovalNumber, aatf.AatfStatus, aatf.Organisation, aatf.Size, aatf.ApprovalDate, aatf.LocalArea, aatf.PanArea);

                var aatf2 = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);
                aatf2.UpdateDetails("AAB", aatf.CompetentAuthority, aatf.ApprovalNumber, aatf.AatfStatus, aatf.Organisation, aatf.Size, aatf.ApprovalDate, aatf.LocalArea, aatf.PanArea);

                var weeeReused1Aatf1 = new WeeeReused(aatf, @return);
                var weeeReused1Aatf2 = new WeeeReused(aatf2, @return);

                foreach (var categoryValue in CategoryValues())
                {
                    db.WeeeContext.WeeeReusedAmount.Add(new Domain.AatfReturn.WeeeReusedAmount(weeeReused1Aatf1, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeReusedAmount.Add(new Domain.AatfReturn.WeeeReusedAmount(weeeReused1Aatf2, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                }

                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf2, @return));
                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 3));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(56);

                foreach (var categoryValue in CategoryValues())
                {
                    var houseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf1[0][TotalReusedHeading].Should().Be(categoryValue.CategoryId);
                    nonHouseHoldResultAatf1[0][TotalReusedHeading].Should().Be((categoryValue.CategoryId + 1));

                    var houseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf2[0][TotalReusedHeading].Should().Be(categoryValue.CategoryId);
                    nonHouseHoldResultAatf2[0][TotalReusedHeading].Should().Be((categoryValue.CategoryId + 1));
                }

                results.Dispose();
            }
        }

        [Fact]
        public async Task Execute_GivenAatfsWithReusedAndCreatedReturn_DataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var @return = SetupCreatedReturn(db);
                var aatf = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAA");
                var aatf2 = SetupAatfWithApprovalDate(db, DateTime.MinValue, "AAB");

                var weeeReused1Aatf1 = new WeeeReused(aatf, @return);
                var weeeReused1Aatf2 = new WeeeReused(aatf2, @return);

                foreach (var categoryValue in CategoryValues())
                {
                    db.WeeeContext.WeeeReusedAmount.Add(new Domain.AatfReturn.WeeeReusedAmount(weeeReused1Aatf1, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                    db.WeeeContext.WeeeReusedAmount.Add(new Domain.AatfReturn.WeeeReusedAmount(weeeReused1Aatf2, categoryValue.CategoryId, categoryValue.CategoryId, categoryValue.CategoryId + 1));
                }

                db.WeeeContext.Returns.Add(@return);

                await db.WeeeContext.SaveChangesAsync();

                db.WeeeContext.ReturnReportOns.Add(new Domain.AatfReturn.ReturnReportOn(@return.Id, 3));

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(56);

                foreach (var categoryValue in CategoryValues())
                {
                    var houseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf1 = results.Select($"AatfKey='{aatf.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf1[0][TotalReusedHeading].Should().Be(categoryValue.CategoryId);
                    nonHouseHoldResultAatf1[0][TotalReusedHeading].Should().Be((categoryValue.CategoryId + 1));

                    var houseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2C}'");
                    var nonHouseHoldResultAatf2 = results.Select($"AatfKey='{aatf2.Id}' AND CategoryId={categoryValue.CategoryId} AND [Obligation Type]='{B2B}'");

                    houseHoldResultAatf2[0][TotalReusedHeading].Should().Be(categoryValue.CategoryId);
                    nonHouseHoldResultAatf2[0][TotalReusedHeading].Should().Be((categoryValue.CategoryId + 1));
                }

                results.Dispose();
            }
        }

        private void AssertSubmittedRow(DataTable results, Aatf aatf, DatabaseWrapper db, int row, string category, string obligation)
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

        private void AssertCreatedRow(DataTable results, Aatf aatf, int row, string category, string obligation)
        {
            results.Rows[row][ComplianceYear].Should().Be(2019);
            results.Rows[row][Quarter].Should().Be("Q1");
            results.Rows[row][Name].Should().Be(aatf.Name);
            results.Rows[row][ApprovalNumber].Should().Be(aatf.ApprovalNumber);
            results.Rows[row][SubmittedBy].Should().Be(" ");
            results.Rows[row][SubmittedDate].ToString().Should().Be(string.Empty);
            results.Rows[row][Obligation].Should().Be(obligation);
            results.Rows[row][Category].Should().Be(category);
            results.Rows[row][TotalReceivedHeading].ToString().Should().BeEmpty();
            results.Rows[row][TotalReusedHeading].ToString().Should().BeEmpty();
            results.Rows[row][TotalSentOnHeading].ToString().Should().BeEmpty();
        }

        private Return SetupSubmittedReturn(DatabaseWrapper db)
        {
            SystemTime.Freeze(date);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            SystemTime.Unfreeze();
            return @return;
        }

        private Return SetupCreatedReturn(DatabaseWrapper db)
        {
            SystemTime.Freeze(date);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            SystemTime.Unfreeze();
            return @return;
        }

        private Aatf SetupAatfWithApprovalDate(DatabaseWrapper db, DateTime date, string name)
        {
            var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);
            aatf.UpdateDetails(name, aatf.CompetentAuthority, aatf.ApprovalNumber, aatf.AatfStatus, aatf.Organisation, aatf.Size, date, aatf.LocalArea, aatf.PanArea);
            return aatf;
        }

        private CategoryValues<ObligatedCategoryValue> CategoryValues()
        {
            return new CategoryValues<ObligatedCategoryValue>();
        }
    }
}
