namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Producer.Classification;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
                Assert.Single(results);

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
                Domain.Organisation.Organisation organisation =
                    Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                Domain.UKCompetentAuthority authority =
                    wrapper.WeeeContext.UKCompetentAuthorities.Single(c =>
                        c.Abbreviation == UKCompetentAuthorityAbbreviationType.EA);
                Domain.Lookup.ChargeBandAmount chargeBandAmount = wrapper.WeeeContext.ChargeBandAmounts.First();
                Quarter quarter = new Quarter(2099, QuarterType.Q1);

                wrapper.WeeeContext.Organisations.Add(organisation);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Arrange - Scheme 1

                Domain.Scheme.Scheme scheme1 = new Domain.Scheme.Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846",
                    Domain.Obligation.ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.Producer.RegisteredProducer registeredProducer1 =
                    new Domain.Producer.RegisteredProducer("WEE/AG48365JN", 2099, scheme1);

                Domain.Scheme.MemberUpload memberUpload1 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    2099,
                    scheme1,
                    "file name",
                    null,
                    false);

                Domain.Producer.ProducerSubmission submission1 = new Domain.Producer.ProducerSubmission(
                    registeredProducer1, memberUpload1,
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
                    0,
                    A.Dummy<StatusType>());

                memberUpload1.ProducerSubmissions.Add(submission1);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload1);
                await wrapper.WeeeContext.SaveChangesAsync();

                registeredProducer1.SetCurrentSubmission(submission1);
                await wrapper.WeeeContext.SaveChangesAsync();

                Domain.DataReturns.DataReturn dataReturn1 = new Domain.DataReturns.DataReturn(scheme1, quarter);

                Domain.DataReturns.DataReturnVersion version1 = new Domain.DataReturns.DataReturnVersion(dataReturn1);

                Domain.DataReturns.EeeOutputAmount amount1 = new Domain.DataReturns.EeeOutputAmount(
                    Domain.Obligation.ObligationType.B2C,
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
                scheme2.UpdateScheme("Test Scheme 2", "WEE/ZU6355HV/SCH", "WEE5746395",
                    Domain.Obligation.ObligationType.B2C, authority);
                scheme2.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.Producer.RegisteredProducer registeredProducer2 =
                    new Domain.Producer.RegisteredProducer("WEE/HT7483HD", 2099, scheme2);

                Domain.Scheme.MemberUpload memberUpload2 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    2099,
                    scheme2,
                    "file name",
                    null,
                    false);

                Domain.Producer.ProducerSubmission submission2 = new Domain.Producer.ProducerSubmission(
                    registeredProducer2, memberUpload2,
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
                    0,
                    A.Dummy<StatusType>());

                memberUpload2.ProducerSubmissions.Add(submission2);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload2);
                await wrapper.WeeeContext.SaveChangesAsync();

                registeredProducer2.SetCurrentSubmission(submission2);
                await wrapper.WeeeContext.SaveChangesAsync();

                Domain.DataReturns.DataReturn dataReturn2 = new Domain.DataReturns.DataReturn(scheme2, quarter);

                Domain.DataReturns.DataReturnVersion version2 = new Domain.DataReturns.DataReturnVersion(dataReturn2);

                Domain.DataReturns.EeeOutputAmount amount2 = new Domain.DataReturns.EeeOutputAmount(
                    Domain.Obligation.ObligationType.B2C,
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
                Domain.Organisation.Organisation organisation =
                    Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                Domain.UKCompetentAuthority authority =
                    wrapper.WeeeContext.UKCompetentAuthorities.Single(c =>
                        c.Abbreviation == UKCompetentAuthorityAbbreviationType.EA);
                Domain.Lookup.ChargeBandAmount chargeBandAmount = wrapper.WeeeContext.ChargeBandAmounts.First();
                Quarter quarter = new Quarter(2099, QuarterType.Q1);

                wrapper.WeeeContext.Organisations.Add(organisation);
                await wrapper.WeeeContext.SaveChangesAsync();

                // Arrange - Scheme 1

                Domain.Scheme.Scheme scheme1 = new Domain.Scheme.Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846",
                    Domain.Obligation.ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.Producer.RegisteredProducer registeredProducer1 =
                    new Domain.Producer.RegisteredProducer("WEE/AG48365JN", 2099, scheme1);

                Domain.Scheme.MemberUpload memberUpload1 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    2099,
                    scheme1,
                    "file name",
                    null,
                    false);

                Domain.Producer.ProducerSubmission submission1 = new Domain.Producer.ProducerSubmission(
                    registeredProducer1, memberUpload1,
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
                    0,
                    A.Dummy<StatusType>());

                memberUpload1.ProducerSubmissions.Add(submission1);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload1);
                await wrapper.WeeeContext.SaveChangesAsync();

                registeredProducer1.SetCurrentSubmission(submission1);
                await wrapper.WeeeContext.SaveChangesAsync();

                Domain.DataReturns.DataReturn dataReturn1 = new Domain.DataReturns.DataReturn(scheme1, quarter);

                Domain.DataReturns.DataReturnVersion version1 = new Domain.DataReturns.DataReturnVersion(dataReturn1);

                Domain.DataReturns.EeeOutputAmount amount1 = new Domain.DataReturns.EeeOutputAmount(
                    Domain.Obligation.ObligationType.B2C,
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
                scheme2.UpdateScheme("Test Scheme 2", "WEE/ZU6355HV/SCH", "WEE5746395",
                    Domain.Obligation.ObligationType.B2C, authority);
                scheme2.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                Domain.Producer.RegisteredProducer registeredProducer2 =
                    new Domain.Producer.RegisteredProducer("WEE/HT7483HD", 2099, scheme2);

                Domain.Scheme.MemberUpload memberUpload2 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    2099,
                    scheme2,
                    "file name",
                    null,
                    false);

                Domain.Producer.ProducerSubmission submission2 = new Domain.Producer.ProducerSubmission(
                    registeredProducer2, memberUpload2,
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
                    0,
                    A.Dummy<StatusType>());

                memberUpload2.ProducerSubmissions.Add(submission2);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload2);
                await wrapper.WeeeContext.SaveChangesAsync();

                registeredProducer2.SetCurrentSubmission(submission2);
                await wrapper.WeeeContext.SaveChangesAsync();

                Domain.DataReturns.DataReturn dataReturn2 = new Domain.DataReturns.DataReturn(scheme2, quarter);

                Domain.DataReturns.DataReturnVersion version2 = new Domain.DataReturns.DataReturnVersion(dataReturn2);

                Domain.DataReturns.EeeOutputAmount amount2 = new Domain.DataReturns.EeeOutputAmount(
                    Domain.Obligation.ObligationType.B2C,
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

                Assert.Single(results);

                Assert.NotNull(results[0]);
                Assert.Equal("Test Scheme 1", results[0].SchemeName);
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_ReturnsResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                var complianceYear = 2060;
                // Direct registrant data is for the previous year
                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear - 1);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2B }, // Should be excluded as B2B
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, amounts1, DirectProducerSubmissionStatus.Complete);

                var (organisation2, directRegistrant2, registeredProducer2) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 2", "WEE/AG48365JX", complianceYear - 1);

                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 4.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission2 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant2, registeredProducer2, complianceYear + 1, amounts2, DirectProducerSubmissionStatus.Complete);

                // Create a scheme for test or ordering
                var organisation =
                    Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                var authority =
                    wrapper.WeeeContext.UKCompetentAuthorities.Single(c =>
                        c.Abbreviation == UKCompetentAuthorityAbbreviationType.EA);
                var chargeBandAmount = wrapper.WeeeContext.ChargeBandAmounts.First();
                var quarter = new Quarter(complianceYear, QuarterType.Q1);

                wrapper.WeeeContext.Organisations.Add(organisation);
                await wrapper.WeeeContext.SaveChangesAsync();

                var scheme1 = new Domain.Scheme.Scheme(organisation);
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453NF/SCH", "WEE9462846",
                    Domain.Obligation.ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                var schemeRegisteredProducer1 =
                    new Domain.Producer.RegisteredProducer("WEE/AG48365JE", complianceYear, scheme1);

                var memberUpload1 = new Domain.Scheme.MemberUpload(
                    organisation.Id,
                    "data",
                    new List<Domain.Scheme.MemberUploadError>(),
                    0,
                    complianceYear,
                    scheme1,
                    "file name",
                    null,
                    false);

                var schemeSubmission1 = new Domain.Producer.ProducerSubmission(
                    schemeRegisteredProducer1, memberUpload1,
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
                    0,
                    A.Dummy<StatusType>());

                memberUpload1.ProducerSubmissions.Add(schemeSubmission1);

                wrapper.WeeeContext.MemberUploads.Add(memberUpload1);
                await wrapper.WeeeContext.SaveChangesAsync();

                schemeRegisteredProducer1.SetCurrentSubmission(schemeSubmission1);
                await wrapper.WeeeContext.SaveChangesAsync();

                var dataReturn1 = new Domain.DataReturns.DataReturn(scheme1, quarter);

                var version1 = new Domain.DataReturns.DataReturnVersion(dataReturn1);

                var amount1 = new Domain.DataReturns.EeeOutputAmount(
                    Domain.Obligation.ObligationType.B2C,
                    WeeeCategory.LargeHouseholdAppliances,
                    123.457m,
                    schemeRegisteredProducer1);

                version1.EeeOutputReturnVersion.AddEeeOutputAmount(amount1);

                wrapper.WeeeContext.DataReturnVersions.Add(version1);
                await wrapper.WeeeContext.SaveChangesAsync();

                dataReturn1.SetCurrentVersion(version1);
                await wrapper.WeeeContext.SaveChangesAsync();

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C");

                results.Should().NotBeNull();
                results.Count.Should().Be(3);

                var schemeElement = results.ElementAt(0);
                schemeElement.SchemeName.Should().Be(scheme1.SchemeName);
                schemeElement.ApprovalNumber.Should().Be(scheme1.ApprovalNumber);
                schemeElement.Cat1Q1.Should().Be(123.457m);
                schemeElement.TotalTonnage.Should().Be(123.457m);
                schemeElement.PRN.Should().Be(schemeRegisteredProducer1.ProducerRegistrationNumber);

                var expectedAmounts1 = new Dictionary<string, decimal> { { "Cat1Q4", 123.456m }, { "Cat4Q4", 2m } };
                AssertEeeElementData(results.ElementAt(1), organisation1, registeredProducer1, country, expectedAmounts1, 125.456m);

                var expectedAmounts2 = new Dictionary<string, decimal> { { "Cat8Q4", 4.456m } };
                AssertEeeElementData(results.ElementAt(2), organisation2, registeredProducer2, country, expectedAmounts2, 4.456m);
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsWithIncorrectStatus_ReturnsEmptyResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var setupData = DirectRegistrantHelper.SetupCommonTestData(wrapper);
                var complianceYear = 2070;

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };
                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant, registeredProducer, complianceYear + 1, amounts, DirectProducerSubmissionStatus.Incomplete);

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C");

                results.Should().NotBeNull();
                results.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsWithNoComplianceYearMatch_ReturnsEmptyResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var complianceYear = 1010;

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", 1000);

                var amounts = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };
                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant, registeredProducer, 1000, amounts, DirectProducerSubmissionStatus.Complete);

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C");

                results.Should().NotBeNull();
                results.Should().BeEmpty();
            }
        }

        private void AssertEeeElementData(
            ProducerEeeCsvData element,
            Domain.Organisation.Organisation organisation,
            Domain.Producer.RegisteredProducer registeredProducer,
            Domain.Country country,
            Dictionary<string, decimal> expectedAmounts,
            decimal expectedTotalTonnage)
        {
            foreach (var pair in expectedAmounts)
            {
                var property = pair.Key;
                var amount = pair.Value;
                element.GetType().GetProperty(property).GetValue(element).Should().Be(amount);
            }

            CheckAllPropertiesAreNull(element, new List<string>(expectedAmounts.Keys));
            element.ProducerCountry.Should().Be(country.Name);
            element.PRN.Should().Be(registeredProducer.ProducerRegistrationNumber);
            element.SchemeName.Should().Be("Direct registrant");
            element.ProducerName.Should().Be(organisation.OrganisationName);
            element.TotalTonnage.Should().Be(expectedTotalTonnage);
        }

        private void CheckAllPropertiesAreNull(ProducerEeeCsvData csvData, List<string> excludeProperties)
        {
            var properties = csvData.GetType().GetProperties();
            if (excludeProperties == null)
            {
                excludeProperties = new List<string>();
            }

            foreach (var property in properties)
            {
                if (property.Name.StartsWith("Cat") && property.Name.Contains("Q") && !excludeProperties.Contains(property.Name))
                {
                    property.GetValue(csvData).Should().BeNull(property.Name + " should be null");
                }
            }
        }
    }
}
