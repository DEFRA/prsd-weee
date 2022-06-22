namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Lookup;
    using EA.Weee.Core.Shared;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using AatfDeliveryLocation = Domain.DataReturns.AatfDeliveryLocation;
    using Assert = Xunit.Assert;
    using Organisation = Domain.Organisation.Organisation;
    using Return = Domain.AatfReturn.Return;
    using Scheme = Domain.Scheme.Scheme;
    using WeeeReceivedAmount = Domain.AatfReturn.WeeeReceivedAmount;

    public class GetPcsAatfDiscrepancyCsvDataTests
    {
        [Fact]
        public async Task Execute_GivenWeeeReceivedData_ReturnsPcsAatfDiscrepancyDataShouldBeCorrect()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Domain.UKCompetentAuthority authority = db.WeeeContext.UKCompetentAuthorities.Single(c => c.Abbreviation == UKCompetentAuthorityAbbreviationType.EA);
                var year = 2019;
                Quarter quarter = new Quarter(year, QuarterType.Q1);

                Scheme scheme1 = new Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846", Domain.Obligation.ObligationType.B2B, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.DataReturns.DataReturn dataReturn1 = new Domain.DataReturns.DataReturn(scheme1, quarter);

                Domain.DataReturns.DataReturnVersion version1 = new Domain.DataReturns.DataReturnVersion(dataReturn1);

                Domain.DataReturns.WeeeDeliveredAmount amount1 = new Domain.DataReturns.WeeeDeliveredAmount(
                    Domain.Obligation.ObligationType.B2B,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m, new AatfDeliveryLocation("WEE/AA1111AA/ATF", string.Empty));

                db.WeeeContext.DataReturns.Add(dataReturn1);
                db.WeeeContext.DataReturnVersions.Add(version1);
                version1.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(amount1);
                await db.WeeeContext.SaveChangesAsync();

                dataReturn1.SetCurrentVersion(version1);
                await db.WeeeContext.SaveChangesAsync();

                var @return = CreateSubmittedReturn(db, organisation);
                var aatf = new Aatf("aatfName", db.WeeeContext.UKCompetentAuthorities.First(), "WEE/AA1111AA/ATF", AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(db), AatfSize.Large, DateTime.Now, ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, year, null, null);

                var weeeReceived = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme1, aatf, @return);
                var weeeReceivedAmounts = new List<WeeeReceivedAmount>()
                {
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 1, 2, 10)
                };

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.WeeeReceived.Add(weeeReceived);
                db.WeeeContext.WeeeReceivedAmount.AddRange(weeeReceivedAmounts);
                await db.WeeeContext.SaveChangesAsync();

                // Act
                var results = await db.StoredProcedures.GetPcsAatfComparisonDataCsvData(year, null, string.Empty);

                var record = results.First(x => x.AatfApprovalNumber == "WEE/AA1111AA/ATF" && x.ObligationType == "B2B");

                //Assert
                Assert.NotNull(record);
                Assert.Equal("WEE/AA1111AA/ATF", record.AatfApprovalNumber);
                Assert.Equal(123.456m, record.PcsTonnage);
                Assert.Equal(10, record.AatfTonnage);
                Assert.Equal(113.456m, record.DifferenceTonnage);
            }
        }

        [Fact]
        public async Task Execute_GivenNoPCSReceivedData_ReturnsPcsAatfDiscrepancyDataShouldBeCorrect()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                Organisation organisation = Organisation.CreateSoleTrader("Test Organisation");
                Domain.UKCompetentAuthority authority = db.WeeeContext.UKCompetentAuthorities.Single(c => c.Abbreviation == UKCompetentAuthorityAbbreviationType.EA);
                var year = 2019;
                Quarter quarter = new Quarter(year, QuarterType.Q1);

                Scheme scheme1 = new Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846", Domain.Obligation.ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                var @return = CreateSubmittedReturn(db, organisation);
                var aatf = new Aatf("aatfName", authority, "WEE/AA1111AA/ATF", AatfStatus.Approved, organisation, AddressHelper.GetAatfAddress(db), AatfSize.Large, DateTime.Now, ObligatedWeeeIntegrationCommon.CreateDefaultContact(db.WeeeContext.Countries.First()), FacilityType.Aatf, year, null, null);

                var weeeReceived = new EA.Weee.Domain.AatfReturn.WeeeReceived(scheme1, aatf, @return);
                var weeeReceivedAmounts = new List<WeeeReceivedAmount>()
                {
                    new Domain.AatfReturn.WeeeReceivedAmount(weeeReceived, 1, 2, 10)
                };

                db.WeeeContext.Returns.Add(@return);
                db.WeeeContext.ReturnAatfs.Add(new ReturnAatf(aatf, @return));
                db.WeeeContext.WeeeReceived.Add(weeeReceived);
                db.WeeeContext.WeeeReceivedAmount.AddRange(weeeReceivedAmounts);
                await db.WeeeContext.SaveChangesAsync();

                // Act
                var results = await db.StoredProcedures.GetPcsAatfComparisonDataCsvData(year, null, null);

                var record = results.First(x => x.AatfApprovalNumber == "WEE/AA1111AA/ATF" && x.ObligationType == "B2B");

                //Assert
                Assert.NotNull(record);
                Assert.Equal("WEE/AA1111AA/ATF", record.AatfApprovalNumber);
                Assert.Equal("WEE/AH7453NF/SCH", record.PcsApprovalNumber);
                Assert.Equal(0, record.PcsTonnage);
                Assert.Equal(10, record.AatfTonnage);
                Assert.Equal(-10, record.DifferenceTonnage);
            }
        }

        private static Return CreateSubmittedReturn(DatabaseWrapper db, Domain.Organisation.Organisation org, bool nilreturn = false)
        {
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(org, db.Model.AspNetUsers.First().Id, FacilityType.Aatf);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, nilreturn);
            return @return;
        }
    }
}
