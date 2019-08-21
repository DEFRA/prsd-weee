﻿namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfReturn;
    using Domain.AatfReturn;
    using Domain.Obligation;
    using FluentAssertions;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using FacilityType = Domain.AatfReturn.FacilityType;
    using Return = Domain.AatfReturn.Return;
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

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(28);

                AssertSubmittedRow(results, aatf, db, 0, "1. Large household appliances", B2C);
                AssertSubmittedRow(results, aatf, db, 1, "2. Small household appliances", B2C);
                AssertSubmittedRow(results, aatf, db, 2, "3. IT and telecommunications equipment", B2C);
                AssertSubmittedRow(results, aatf, db, 3, "4. Consumer equipment", B2C);
                AssertSubmittedRow(results, aatf, db, 4, "5. Lighting equipment", B2C);
                AssertSubmittedRow(results, aatf, db, 5, "6. Electrical and electronic tools", B2C);
                AssertSubmittedRow(results, aatf, db, 6, "7. Toys, leisure and sports equipment", B2C);
                AssertSubmittedRow(results, aatf, db, 7, "8. Medical devices", B2C);
                AssertSubmittedRow(results, aatf, db, 8, "9. Monitoring and control instruments", B2C);
                AssertSubmittedRow(results, aatf, db, 9, "10. Automatic dispensers", B2C);
                AssertSubmittedRow(results, aatf, db, 10, "11. Display equipment", B2C);
                AssertSubmittedRow(results, aatf, db, 11, "12. Appliances containing refrigerants", B2C);
                AssertSubmittedRow(results, aatf, db, 12, "13. Gas discharge lamps and LED light sources", B2C);
                AssertSubmittedRow(results, aatf, db, 13, "14. Photovoltaic panels", B2C);
                AssertSubmittedRow(results, aatf, db, 14, "1. Large household appliances", B2B);
                AssertSubmittedRow(results, aatf, db, 15, "2. Small household appliances", B2B);
                AssertSubmittedRow(results, aatf, db, 16, "3. IT and telecommunications equipment", B2B);
                AssertSubmittedRow(results, aatf, db, 17, "4. Consumer equipment", B2B);
                AssertSubmittedRow(results, aatf, db, 18, "5. Lighting equipment", B2B);
                AssertSubmittedRow(results, aatf, db, 19, "6. Electrical and electronic tools", B2B);
                AssertSubmittedRow(results, aatf, db, 20, "7. Toys, leisure and sports equipment", B2B);
                AssertSubmittedRow(results, aatf, db, 21, "8. Medical devices", B2B);
                AssertSubmittedRow(results, aatf, db, 22, "9. Monitoring and control instruments", B2B);
                AssertSubmittedRow(results, aatf, db, 23, "10. Automatic dispensers", B2B);
                AssertSubmittedRow(results, aatf, db, 24, "11. Display equipment", B2B);
                AssertSubmittedRow(results, aatf, db, 25, "12. Appliances containing refrigerants", B2B);
                AssertSubmittedRow(results, aatf, db, 26, "13. Gas discharge lamps and LED light sources", B2B);
                AssertSubmittedRow(results, aatf, db, 27, "14. Photovoltaic panels", B2B);

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

                var results = await db.StoredProcedures.GetReturnObligatedCsvData(@return.Id);
                results.Rows.Count.Should().Be(28);

                AssertCreatedRow(results, aatf, 0, "1. Large household appliances", B2C);
                AssertCreatedRow(results, aatf, 1, "2. Small household appliances", B2C);
                AssertCreatedRow(results, aatf, 2, "3. IT and telecommunications equipment", B2C);
                AssertCreatedRow(results, aatf, 3, "4. Consumer equipment", B2C);
                AssertCreatedRow(results, aatf, 4, "5. Lighting equipment", B2C);
                AssertCreatedRow(results, aatf, 5, "6. Electrical and electronic tools", B2C);
                AssertCreatedRow(results, aatf, 6, "7. Toys, leisure and sports equipment", B2C);
                AssertCreatedRow(results, aatf, 7, "8. Medical devices", B2C);
                AssertCreatedRow(results, aatf, 8, "9. Monitoring and control instruments", B2C);
                AssertCreatedRow(results, aatf, 9, "10. Automatic dispensers", B2C);
                AssertCreatedRow(results, aatf, 10, "11. Display equipment", B2C);
                AssertCreatedRow(results, aatf, 11, "12. Appliances containing refrigerants", B2C);
                AssertCreatedRow(results, aatf, 12, "13. Gas discharge lamps and LED light sources", B2C);
                AssertCreatedRow(results, aatf, 13, "14. Photovoltaic panels", B2C);
                AssertCreatedRow(results, aatf, 14, "1. Large household appliances", B2B);
                AssertCreatedRow(results, aatf, 15, "2. Small household appliances", B2B);
                AssertCreatedRow(results, aatf, 16, "3. IT and telecommunications equipment", B2B);
                AssertCreatedRow(results, aatf, 17, "4. Consumer equipment", B2B);
                AssertCreatedRow(results, aatf, 18, "5. Lighting equipment", B2B);
                AssertCreatedRow(results, aatf, 19, "6. Electrical and electronic tools", B2B);
                AssertCreatedRow(results, aatf, 20, "7. Toys, leisure and sports equipment", B2B);
                AssertCreatedRow(results, aatf, 21, "8. Medical devices", B2B);
                AssertCreatedRow(results, aatf, 22, "9. Monitoring and control instruments", B2B);
                AssertCreatedRow(results, aatf, 23, "10. Automatic dispensers", B2B);
                AssertCreatedRow(results, aatf, 24, "11. Display equipment", B2B);
                AssertCreatedRow(results, aatf, 25, "12. Appliances containing refrigerants", B2B);
                AssertCreatedRow(results, aatf, 26, "13. Gas discharge lamps and LED light sources", B2B);
                AssertCreatedRow(results, aatf, 27, "14. Photovoltaic panels", B2B);

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

                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf2, @return));
                db.WeeeContext.Returns.Add(@return);

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

                db.WeeeContext.Returns.Add(@return);

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
            return new CategoryValues<EA.Weee.Core.AatfReturn.ObligatedCategoryValue>();
        }
    }
}
