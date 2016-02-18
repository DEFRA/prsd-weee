namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Obligation;
    using FakeItEasy;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgProducerEeeCsvDataTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsProducerEeeWithSelectedYearAndObligationType()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "PRN123");
                var producer2 = helper.CreateProducerAsCompany(memberUpload, "PRN456");

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2000, 2);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2C", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, "B2C", 2, 1000);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer2.RegisteredProducer, "B2B", 2, 400);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeCsvData(2000, null, "B2C");

                //Assert
                Assert.NotNull(results);

                //Data return with obliation type B2B should not be there in the result.
                ProducerEeeCsvData b2bProducer = results.Find(x => (x.PRN == "PRN456"));
                Assert.Null(b2bProducer);

                ProducerEeeCsvData result = results[0];
                Assert.Equal("test scheme name", result.SchemeName);
                Assert.Equal("WEE/TE0000ST/SCH", result.ApprovalNumber);
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal(100, result.Cat1Q1);
                Assert.Equal(1000, result.Cat2Q2);
                Assert.Equal(1100, result.TotalTonnage);
                Assert.Null(result.Cat1Q3);
            }
        }

        [Fact]
        public async Task Execute_ReturnsEeeData_ForNonRemovedProducersOnly()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "PRN123");
                producer1.RegisteredProducer.Removed = true;

                var producer2 = helper.CreateProducerAsCompany(memberUpload, "PRN789");

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2000, 1);

                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2C", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion, producer2.RegisteredProducer, "B2C", 1, 200);

                db.Model.SaveChanges();

                // Act
                var results =
                    await db.StoredProcedures.SpgProducerEeeCsvData(2000, null, "B2C");

                // Assert
                Assert.Equal(1, results.Count);

                var data = results.Single();

                Assert.Equal("PRN789", data.PRN);
                Assert.Equal(200, data.Cat1Q1);
            }
        }

        /// <summary>
        /// This test ensures that when no scheme ID is provided to the stored procedure, results
        /// for all scheme are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithoutSchemeFilter_ReturnsResultsForAllSchemes()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                Domain.Organisation.Organisation organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                Domain.UKCompetentAuthority authority = wrapper.WeeeContext.UKCompetentAuthorities.Single(c => c.Abbreviation == "EA");
                Domain.Lookup.ChargeBandAmount chargeBandAmount = wrapper.WeeeContext.ChargeBandAmounts.First();
                Quarter quarter = new Quarter(2099, QuarterType.Q1);

                wrapper.WeeeContext.Organisations.Add(organisation);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Arrange - Scheme 1

                Domain.Scheme.Scheme scheme1 = new Domain.Scheme.Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846", ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.Producer.RegisteredProducer registeredProducer1 = new Domain.Producer.RegisteredProducer("WEE/AG48365JN", 2099, scheme1);

                Domain.Scheme.MemberUpload memberUpload1 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    2099,
                    scheme1,
                    "file name");

                Domain.Producer.ProducerSubmission submission1 = new Domain.Producer.ProducerSubmission(registeredProducer1, memberUpload1,
                    new Domain.Producer.ProducerBusiness(),
                    new Domain.Producer.AuthorisedRepresentative("Foo"),
                    new DateTime(2016, 1, 1),
                    0,
                    true,
                    null,
                    "Trading Name 1",
                    Domain.Producer.Classfication.EEEPlacedOnMarketBandType.Both,
                    Domain.Producer.Classfication.SellingTechniqueType.Both,
                    Domain.Obligation.ObligationType.B2C,
                    Domain.Producer.Classfication.AnnualTurnOverBandType.Lessthanorequaltoonemillionpounds,
                    new List<Domain.Producer.BrandName>(),
                    new List<Domain.Producer.SICCode>(),
                    chargeBandAmount,
                    0);

                memberUpload1.ProducerSubmissions.Add(submission1);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload1);
                await wrapper.WeeeContext.SaveChangesAsync();

                registeredProducer1.SetCurrentSubmission(submission1);
                await wrapper.WeeeContext.SaveChangesAsync();

                Domain.DataReturns.DataReturn dataReturn1 = new Domain.DataReturns.DataReturn(scheme1, quarter);

                Domain.DataReturns.DataReturnVersion version1 = new Domain.DataReturns.DataReturnVersion(dataReturn1);

                Domain.DataReturns.EeeOutputAmount amount1 = new Domain.DataReturns.EeeOutputAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    registeredProducer1);

                version1.EeeOutputReturnVersion.AddEeeOutputAmount(amount1);

                wrapper.WeeeContext.DataReturnVersions.Add(version1);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn1.SetCurrentVersion(version1);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Arrange - Scheme 2

                Domain.Scheme.Scheme scheme2 = new Domain.Scheme.Scheme(organisation);
                scheme2.UpdateScheme("Test Scheme 2", "WEE/ZU6355HV/SCH", "WEE5746395", ObligationType.B2C, authority);
                scheme2.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.Producer.RegisteredProducer registeredProducer2 = new Domain.Producer.RegisteredProducer("WEE/HT7483HD", 2099, scheme2);

                Domain.Scheme.MemberUpload memberUpload2 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    2099,
                    scheme2,
                    "file name");

                Domain.Producer.ProducerSubmission submission2 = new Domain.Producer.ProducerSubmission(registeredProducer2, memberUpload2,
                    new Domain.Producer.ProducerBusiness(),
                    new Domain.Producer.AuthorisedRepresentative("Foo"),
                    new DateTime(2016, 1, 1),
                    0,
                    true,
                    null,
                    "Trading Name 2",
                    Domain.Producer.Classfication.EEEPlacedOnMarketBandType.Both,
                    Domain.Producer.Classfication.SellingTechniqueType.Both,
                    Domain.Obligation.ObligationType.B2C,
                    Domain.Producer.Classfication.AnnualTurnOverBandType.Lessthanorequaltoonemillionpounds,
                    new List<Domain.Producer.BrandName>(),
                    new List<Domain.Producer.SICCode>(),
                    chargeBandAmount,
                    0);

                memberUpload2.ProducerSubmissions.Add(submission2);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload2);
                await wrapper.WeeeContext.SaveChangesAsync();

                registeredProducer2.SetCurrentSubmission(submission2);
                await wrapper.WeeeContext.SaveChangesAsync();

                Domain.DataReturns.DataReturn dataReturn2 = new Domain.DataReturns.DataReturn(scheme2, quarter);

                Domain.DataReturns.DataReturnVersion version2 = new Domain.DataReturns.DataReturnVersion(dataReturn2);

                Domain.DataReturns.EeeOutputAmount amount2 = new Domain.DataReturns.EeeOutputAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    registeredProducer2);

                version2.EeeOutputReturnVersion.AddEeeOutputAmount(amount2);

                wrapper.WeeeContext.DataReturnVersions.Add(version2);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn2.SetCurrentVersion(version2);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                List<ProducerEeeCsvData> results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(
                    2099,
                    null,
                    "B2C");

                // Assert
                Assert.NotNull(results);

                Assert.Equal(2, results.Count);
            }
        }

        /// <summary>
        /// This test ensures that when a scheme ID is provided to the stored procedure, only results
        /// for that scheme are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSchemeFilter_ReturnsResultsForSelectedSchemeOnly()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                Domain.Organisation.Organisation organisation = Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                Domain.UKCompetentAuthority authority = wrapper.WeeeContext.UKCompetentAuthorities.Single(c => c.Abbreviation == "EA");
                Domain.Lookup.ChargeBandAmount chargeBandAmount = wrapper.WeeeContext.ChargeBandAmounts.First();
                Quarter quarter = new Quarter(2099, QuarterType.Q1);

                wrapper.WeeeContext.Organisations.Add(organisation);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Arrange - Scheme 1

                Domain.Scheme.Scheme scheme1 = new Domain.Scheme.Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846", ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.Producer.RegisteredProducer registeredProducer1 = new Domain.Producer.RegisteredProducer("WEE/AG48365JN", 2099, scheme1);

                Domain.Scheme.MemberUpload memberUpload1 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    2099,
                    scheme1,
                    "file name");

                Domain.Producer.ProducerSubmission submission1 = new Domain.Producer.ProducerSubmission(registeredProducer1, memberUpload1,
                    new Domain.Producer.ProducerBusiness(),
                    new Domain.Producer.AuthorisedRepresentative("Foo"),
                    new DateTime(2016, 1, 1),
                    0,
                    true,
                    null,
                    "Trading Name 2",
                    Domain.Producer.Classfication.EEEPlacedOnMarketBandType.Both,
                    Domain.Producer.Classfication.SellingTechniqueType.Both,
                    Domain.Obligation.ObligationType.B2C,
                    Domain.Producer.Classfication.AnnualTurnOverBandType.Lessthanorequaltoonemillionpounds,
                    new List<Domain.Producer.BrandName>(),
                    new List<Domain.Producer.SICCode>(),
                    chargeBandAmount,
                    0);

                memberUpload1.ProducerSubmissions.Add(submission1);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload1);
                await wrapper.WeeeContext.SaveChangesAsync();

                registeredProducer1.SetCurrentSubmission(submission1);
                await wrapper.WeeeContext.SaveChangesAsync();

                Domain.DataReturns.DataReturn dataReturn1 = new Domain.DataReturns.DataReturn(scheme1, quarter);

                Domain.DataReturns.DataReturnVersion version1 = new Domain.DataReturns.DataReturnVersion(dataReturn1);

                Domain.DataReturns.EeeOutputAmount amount1 = new Domain.DataReturns.EeeOutputAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    registeredProducer1);

                version1.EeeOutputReturnVersion.AddEeeOutputAmount(amount1);

                wrapper.WeeeContext.DataReturnVersions.Add(version1);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn1.SetCurrentVersion(version1);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Arrange - Scheme 2

                Domain.Scheme.Scheme scheme2 = new Domain.Scheme.Scheme(organisation);
                scheme2.UpdateScheme("Test Scheme 2", "WEE/ZU6355HV/SCH", "WEE5746395", ObligationType.B2C, authority);
                scheme2.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.Producer.RegisteredProducer registeredProducer2 = new Domain.Producer.RegisteredProducer("WEE/HT7483HD", 2099, scheme2);

                Domain.Scheme.MemberUpload memberUpload2 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    2099,
                    scheme2,
                    "file name");

                Domain.Producer.ProducerSubmission submission2 = new Domain.Producer.ProducerSubmission(registeredProducer2, memberUpload2,
                    new Domain.Producer.ProducerBusiness(),
                    new Domain.Producer.AuthorisedRepresentative("Foo"),
                    new DateTime(2016, 1, 1),
                    0,
                    true,
                    null,
                    "Trading Name 1",
                    Domain.Producer.Classfication.EEEPlacedOnMarketBandType.Both,
                    Domain.Producer.Classfication.SellingTechniqueType.Both,
                    Domain.Obligation.ObligationType.B2C,
                    Domain.Producer.Classfication.AnnualTurnOverBandType.Lessthanorequaltoonemillionpounds,
                    new List<Domain.Producer.BrandName>(),
                    new List<Domain.Producer.SICCode>(),
                    chargeBandAmount,
                    0);

                memberUpload2.ProducerSubmissions.Add(submission2);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload2);
                await wrapper.WeeeContext.SaveChangesAsync();

                registeredProducer2.SetCurrentSubmission(submission2);
                await wrapper.WeeeContext.SaveChangesAsync();

                Domain.DataReturns.DataReturn dataReturn2 = new Domain.DataReturns.DataReturn(scheme2, quarter);

                Domain.DataReturns.DataReturnVersion version2 = new Domain.DataReturns.DataReturnVersion(dataReturn2);

                Domain.DataReturns.EeeOutputAmount amount2 = new Domain.DataReturns.EeeOutputAmount(
                    ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.456m,
                    registeredProducer2);

                version2.EeeOutputReturnVersion.AddEeeOutputAmount(amount2);

                wrapper.WeeeContext.DataReturnVersions.Add(version2);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn2.SetCurrentVersion(version2);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                List<ProducerEeeCsvData> results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(
                    2099,
                    scheme1.Id,
                    "B2C");

                // Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.Count);

                Assert.NotNull(results[0]);
                Assert.Equal("Test Scheme 1", results[0].SchemeName);
            }
        }
    }
}
