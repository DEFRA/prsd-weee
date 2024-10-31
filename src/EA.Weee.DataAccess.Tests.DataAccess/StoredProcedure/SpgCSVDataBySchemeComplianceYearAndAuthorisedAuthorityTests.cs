namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using EA.Prsd.Core;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Domain.Producer.Classification;
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

    public class SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthorityTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsProducerWithSelectedSchemeandAA()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.CompetentAuthorityId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results =
                    await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false,
                        false, scheme1.Id, scheme1.CompetentAuthorityId, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                MembersDetailsCsvData result = results[0];

                Assert.Equal("WEE/11AAAA11", result.PRN);
            }
        }

        /// <summary>
        /// This test ensures that a producer in the database associated with a
        /// member upload that has not yet been submitted will not appear in the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithNonSubmittedMemberUpload_IgnoresProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = false;

                helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        /// <summary>
        /// This test ensures that for a producer which is a company, the company name
        /// is returned as the OrganisationName property of the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentProducerAsCompany_ReturnsCompanyName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                MembersDetailsCsvData result = results[0];

                Assert.Equal(producer1.Business.Company.Name, result.ProducerName);
            }
        }

        /// <summary>
        /// This test ensures that for a producer which is a partnership, the partnership name
        /// is returned as the OrganisationName property of the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentProducerAsPartnership_ReturnsPartnershipName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                MembersDetailsCsvData result = results[0];

                Assert.Equal(producer1.Business.Partnership.Name, result.ProducerName);
            }
        }

        /// <summary>
        /// This test ensures that for a producer which is a sole trader, an empty string
        /// is returned as the OrganisationName property of the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentProducerAsSoleTrader_ReturnsNoName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsSoleTrader(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                MembersDetailsCsvData result = results[0];

                Assert.Null(result.ProducerName);
            }
        }

        /// <summary>
        /// This test ensures that only producers registered in the specified compliance year
        /// are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentInSeveralYearsProducer_ReturnsTheCorrectYearsData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);
                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                MembersDetailsCsvData result = results[0];

                Assert.Equal(producer1.Business.Partnership.Name, result.ProducerName);
            }
        }

        /// <summary>
        /// This test ensures that only producers registered in the specified scheme are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithProducersInOtherSchemes_ReturnsOtherSchemesProducers()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                Scheme scheme2 = helper.CreateScheme();

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/22BBBB22");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, null, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.Any(r => r.PRN == "WEE/11AAAA11"), "Producers from both schemes should be returned when no scheme ID is specified.");
                Assert.True(results.Any(r => r.PRN == "WEE/22BBBB22"), "Producers from both schemes should be returned when no scheme ID is specified.");
            }
        }

        /// <summary>
        /// This test ensures that the results are ordered by organisation name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralProducers_ReturnsResultsOrderedByOrganisationName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";

                Weee.Tests.Core.Model.ProducerSubmission producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);

                Assert.Collection(results,
                    (r1) => Assert.Equal("AAAA", r1.ProducerName),
                    (r2) => Assert.Equal("ABCD", r2.ProducerName),
                    (r3) => Assert.Equal("ABCH", r3.ProducerName));
            }
        }

        /// <summary>
        /// This test ensures that the results contains removed producers
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralProducersAndIncludeRemovedProducersIsYes_ReturnsAllProducersWithRemovedProducers()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";
                producer1.RegisteredProducer.Removed = true;

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";
                producer2.RegisteredProducer.Removed = true;

                Weee.Tests.Core.Model.ProducerSubmission producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, true, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(3, results.Count);
                Assert.Collection(results,
                    (r1) => Assert.Equal("WEE/22AAAA22", r1.PRN),
                    (r2) => Assert.Equal("WEE/33CCCC33", r2.PRN),
                    (r3) => Assert.Equal("WEE/11BBBB11", r3.PRN));
            }
        }

        /// <summary>
        /// This test ensures that the results contains only non removed producers
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralProducersAndIncludeRemovedProducersIsNo_ReturnsAllProducersWithoutRemovedProducers()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";
                producer1.RegisteredProducer.Removed = true;

                Weee.Tests.Core.Model.ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";
                producer2.RegisteredProducer.Removed = true;

                Weee.Tests.Core.Model.ProducerSubmission producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";
                producer3.RegisteredProducer.Removed = false;

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);

                Assert.Single(results);
                Assert.Collection(results,
                   (r1) => Assert.Equal("WEE/33CCCC33", r1.PRN));
            }
        }

        [Fact]
        public async Task Execute_WithoutProducerBrandNames_ReturnsBrandNamesAsNull()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;
                memberUpload.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer = helper.CreateProducerAsPartnership(memberUpload, "WEE/11BBBB11");
                producer.Business.Partnership.Name = "ABCH";
                producer.RegisteredProducer.Removed = false;

                helper.CreateBrandName(producer, "Brand1");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                Assert.Null(results[0].BrandNames);
            }
        }

        [Fact]
        public async Task Execute_WithProducerBrandNames_ReturnsBrandNames()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;
                memberUpload.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer = helper.CreateProducerAsPartnership(memberUpload, "WEE/11BBBB11");
                producer.Business.Partnership.Name = "ABCH";
                producer.RegisteredProducer.Removed = false;

                helper.CreateBrandName(producer, "Brand1");
                helper.CreateBrandName(producer, "Brand2");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, true, scheme.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                Assert.NotNull(results[0].BrandNames);
                Assert.Equal("Brand1; Brand2", results[0].BrandNames);
            }
        }

        [Fact]
        public async Task Execute_WithProducerBrandNames_ReturnsBrandNamesForRemovedProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;
                memberUpload.SubmittedDate = new DateTime(2015, 1, 1);

                Weee.Tests.Core.Model.ProducerSubmission producer = helper.CreateProducerAsPartnership(memberUpload, "WEE/11BBBB11");
                producer.Business.Partnership.Name = "ABCH";
                producer.RegisteredProducer.Removed = true;

                helper.CreateBrandName(producer, "Brand1");
                helper.CreateBrandName(producer, "Brand2");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCsvData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, true, true, scheme.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                Assert.NotNull(results[0].BrandNames);
                Assert.Equal("Brand1; Brand2", results[0].BrandNames);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Execute_WithDirectRegistrantSubmissions_ReturnsResults(bool filterDirectRegistrant)
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                var complianceYear = 2080;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, amounts1, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                var authorisedRep = new Domain.Producer.AuthorisedRepresentative("authed rep name",
                    new ProducerContact("rep title", "rep first name", "rep surname",
                        "rep tel", "rep mob", "rep fax", "rep email", new ProducerAddress("rep address1",
                            "rep secondary", "rep street",
                            "rep town", "rep locality", "rep admin area", country, "rep postcode")));

                var brandNames = new BrandName("brand name");
                var (_, directRegistrant2, registeredProducer2) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 2", "WEE/AG48365JX", complianceYear,  "987654321", authorisedRep, brandNames);

                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 4.456m, ObligationType = Domain.Obligation.ObligationType.B2B }
                };

                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant2, registeredProducer2, complianceYear, amounts2, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.IndirectSellingtoEndUser.Value);

                // should include removed
                registeredProducer2.Remove();
                
                await wrapper.WeeeContext.SaveChangesAsync();

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

                memberUpload1.SetSubmittedDate(SystemTime.UtcNow);
                memberUpload1.Submit(wrapper.WeeeContext.Users.First());

                var schemeSubmission1 = new Domain.Producer.ProducerSubmission(
                    schemeRegisteredProducer1, memberUpload1,
                    new Domain.Producer.ProducerBusiness(),
                    new Domain.Producer.AuthorisedRepresentative("Foo"),
                    new DateTime(complianceYear, 1, 1),
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

                List<MembersDetailsCsvData> results = await wrapper.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(complianceYear, true, true, null, null, filterDirectRegistrant);

                var assertionSmallProducerStartIndex = 1;
                if (filterDirectRegistrant)
                {
                    assertionSmallProducerStartIndex = 0;
                }
                else
                {
                    var schemeResult = results.ElementAt(0);
                    schemeResult.ApprovalNumber.Should().Be("WEE/AH7453NF/SCH");
                }

                var result1 = results.ElementAt(assertionSmallProducerStartIndex);
                result1.CompanyName.Should().Be("My company 2");
                result1.SchemeName.Should().Be("Direct registrant");
                result1.TradingName.Should().BeNullOrWhiteSpace();
                result1.ProducerType.Should().Be("Registered company");
                result1.ProducerName.Should().Be("authed rep name");
                result1.PRN.Should().Be("WEE/AG48365JX");
                result1.SICCodes.Should().BeNull();
                result1.VATRegistered.Should().BeNull();
                result1.AnnualTurnover.Should().BeNull();
                result1.AnnualTurnoverBandType.Should().BeNull();
                result1.EEEPlacedOnMarketBandType.Should().Be("Less than 5T EEE placed on market");
                result1.ObligationType.Should().Be("B2B");
                result1.ChargeBandType.Should().BeNull();
                result1.SellingTechniqueType.Should().Be("Indirect Selling to End User");
                result1.CeaseToExist.Should().BeNull();
                result1.CNTitle.Should().BeNull();
                result1.CNForename.Should().BeNull();
                result1.CNSurname.Should().BeNull();
                result1.CNTelephone.Should().BeNull();
                result1.CNMobile.Should().BeNull();
                result1.CNFax.Should().BeNull();
                result1.CNEmail.Should().BeNull();
                result1.CNPrimaryName.Should().BeNull();
                result1.CNSecondaryName.Should().BeNull();
                result1.CNStreet.Should().BeNull();
                result1.CNTown.Should().BeNull();
                result1.CNLocality.Should().BeNull();
                result1.CNAdministrativeArea.Should().BeNull();
                result1.CNPostcode.Should().BeNull();
                result1.CNCountry.Should().BeNull();
                result1.CNCountry.Should().BeNull();
                result1.CompanyNumber.Should().Be("123456789");
                result1.CompanyContactTitle.Should().BeNull();
                result1.CompanyContactForename.Should().Be("first name");
                result1.CompanyContactSurname.Should().Be("last name");
                result1.CompanyContactTelephone.Should().Be("12345678");
                result1.CompanyContactMobile.Should().BeNull();
                result1.CompanyContactFax.Should().BeNull();
                result1.CompanyContactCountry.Should().Be("Azerbaijan");
                result1.CompanyContactEmail.Should().Be("test@co.uk");
                result1.CompanyContactPrimaryName.Should().Be("primary 1");
                result1.CompanyContactSecondaryName.Should().BeNull();
                result1.CompanyContactStreet.Should().Be("street");
                result1.CompanyContactTown.Should().Be("Woking");
                result1.CompanyContactLocality.Should().Be("Hampshire");
                result1.CompanyContactAdministrativeArea.Should().BeNull();
                result1.CompanyContactPostcode.Should().Be("GU21 5EE");
                result1.CompanyContactCountry.Should().Be("Azerbaijan");
                result1.PPOBContactTitle.Should().BeNull();
                result1.PPOBContactForename.Should().BeNull();
                result1.PPOBContactSurname.Should().BeNull();
                result1.PPOBContactTelephone.Should().BeNull();
                result1.PPOBContactMobile.Should().BeNull();
                result1.PPOBContactFax.Should().BeNull();
                result1.PPOBContactEmail.Should().BeNull();
                result1.PPOBContactPrimaryName.Should().BeNull();
                result1.PPOBContactSecondaryName.Should().BeNull();
                result1.PPOBContactStreet.Should().BeNull();
                result1.PPOBContactTown.Should().BeNull();
                result1.PPOBContactLocality.Should().BeNull();
                result1.PPOBContactAdministrativeArea.Should().BeNull();
                result1.PPOBContactPostcode.Should().BeNull();
                result1.OverseasProducerName.Should().Be("authed rep name");
                result1.OverseasContactForename.Should().Be("rep first name");
                result1.OverseasContactSurname.Should().Be("rep surname");
                result1.OverseasContactTelephone.Should().Be("rep tel");
                result1.OverseasContactMobile.Should().Be("rep mob");
                result1.OverseasContactFax.Should().Be("rep fax");
                result1.OverseasContactEmail.Should().Be("rep email");
                result1.OverseasContactPrimaryName.Should().Be("rep address1");
                result1.OverseasContactSecondaryName.Should().Be("rep secondary");
                result1.OverseasContactStreet.Should().Be("rep street");
                result1.OverseasContactTown.Should().Be("rep town");
                result1.OverseasContactLocality.Should().Be("rep locality");
                result1.OverseasContactAdministrativeArea.Should().Be("rep admin area");
                result1.OverseasContactPostcode.Should().Be("rep postcode");
                result1.OverseasContactCountry.Should().Be("Azerbaijan");
                result1.RemovedFromScheme.Should().Be("Yes");
                result1.DateAmended.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromMinutes(2));
                result1.DateRegistered.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromMinutes(2));
                result1.BrandNames.Should().Be("brand name");

                var result2 = results.ElementAt(assertionSmallProducerStartIndex + 1);
                result2.CompanyName.Should().Be("My company");
                result2.SchemeName.Should().Be("Direct registrant");
                result2.TradingName.Should().BeNullOrWhiteSpace();
                result2.ProducerType.Should().Be("Registered company");
                result2.ProducerName.Should().Be("My company");
                result2.PRN.Should().Be("WEE/AG48365JN");
                result2.SICCodes.Should().BeNull();
                result2.VATRegistered.Should().BeNull();
                result2.AnnualTurnover.Should().BeNull();
                result2.AnnualTurnoverBandType.Should().BeNull();
                result2.EEEPlacedOnMarketBandType.Should().Be("Less than 5T EEE placed on market");
                result2.ObligationType.Should().Be("B2C");
                result2.ChargeBandType.Should().BeNull();
                result2.SellingTechniqueType.Should().Be("Both");
                result2.CeaseToExist.Should().BeNull();
                result2.CNTitle.Should().BeNull();
                result2.CNForename.Should().BeNull();
                result2.CNSurname.Should().BeNull();
                result2.CNTelephone.Should().BeNull();
                result2.CNMobile.Should().BeNull();
                result2.CNFax.Should().BeNull();
                result2.CNEmail.Should().BeNull();
                result2.CNPrimaryName.Should().BeNull();
                result2.CNSecondaryName.Should().BeNull();
                result2.CNStreet.Should().BeNull();
                result2.CNTown.Should().BeNull();
                result2.CNLocality.Should().BeNull();
                result2.CNAdministrativeArea.Should().BeNull();
                result2.CNPostcode.Should().BeNull();
                result2.CNCountry.Should().BeNull();
                result2.CNCountry.Should().BeNull();
                result2.CompanyNumber.Should().Be("123456789");
                result2.CompanyContactTitle.Should().BeNull();
                result2.CompanyContactForename.Should().Be("first name");
                result2.CompanyContactSurname.Should().Be("last name");
                result2.CompanyContactTelephone.Should().Be("12345678");
                result2.CompanyContactMobile.Should().BeNull();
                result2.CompanyContactFax.Should().BeNull();
                result2.CompanyContactCountry.Should().Be("Azerbaijan");
                result2.CompanyContactEmail.Should().Be("test@co.uk");
                result2.CompanyContactPrimaryName.Should().Be("primary 1");
                result2.CompanyContactSecondaryName.Should().BeNull();
                result2.CompanyContactStreet.Should().Be("street");
                result2.CompanyContactTown.Should().Be("Woking");
                result2.CompanyContactLocality.Should().Be("Hampshire");
                result2.CompanyContactAdministrativeArea.Should().BeNull();
                result2.CompanyContactPostcode.Should().Be("GU21 5EE");
                result2.CompanyContactCountry.Should().Be("Azerbaijan");
                result2.PPOBContactTitle.Should().BeNull();
                result2.PPOBContactForename.Should().BeNull();
                result2.PPOBContactSurname.Should().BeNull();
                result2.PPOBContactTelephone.Should().BeNull();
                result2.PPOBContactMobile.Should().BeNull();
                result2.PPOBContactFax.Should().BeNull();
                result2.PPOBContactEmail.Should().BeNull();
                result2.PPOBContactPrimaryName.Should().BeNull();
                result2.PPOBContactSecondaryName.Should().BeNull();
                result2.PPOBContactStreet.Should().BeNull();
                result2.PPOBContactTown.Should().BeNull();
                result2.PPOBContactLocality.Should().BeNull();
                result2.PPOBContactAdministrativeArea.Should().BeNull();
                result2.PPOBContactPostcode.Should().BeNull();
                result2.OverseasProducerName.Should().BeNull();
                result2.OverseasContactForename.Should().BeNull();
                result2.OverseasContactSurname.Should().BeNull();
                result2.OverseasContactTelephone.Should().BeNull();
                result2.OverseasContactMobile.Should().BeNull();
                result2.OverseasContactFax.Should().BeNull();
                result2.OverseasContactEmail.Should().BeNull();
                result2.OverseasContactPrimaryName.Should().BeNull();
                result2.OverseasContactSecondaryName.Should().BeNull();
                result2.OverseasContactStreet.Should().BeNull();
                result2.OverseasContactTown.Should().BeNull();
                result2.OverseasContactLocality.Should().BeNull();
                result2.OverseasContactAdministrativeArea.Should().BeNull();
                result2.OverseasContactPostcode.Should().BeNull();
                result2.OverseasContactCountry.Should().BeNull();
                result2.RemovedFromScheme.Should().Be("No");
                result2.DateAmended.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromMinutes(2));
                result2.DateRegistered.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromMinutes(2));
                result2.BrandNames.Should().BeNullOrWhiteSpace();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_WithExcludeRemovedProducer_ShouldReturnNoResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, _) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2080;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, amounts1, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                registeredProducer1.Remove();

                await wrapper.WeeeContext.SaveChangesAsync();

                var results = await wrapper.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(complianceYear, false, true, null, null, false);

                results.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_WithReturnedSubmission_ShouldUseRecentSubmittedData()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2082;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG43365JN", complianceYear);
                
                // initially no EEE and selling technique type of both
                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                // return the submission 
                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission1);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                // resubmit with EEE and update the selling technique, this data should be the data returned as its most recent
                await DirectRegistrantHelper.SubmitSubmission(wrapper, submission1,
                    amounts1,
                    SellingTechniqueType.DirectSellingtoEndUser.Value);

                await wrapper.WeeeContext.SaveChangesAsync();

                var results = await wrapper.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(complianceYear, true, true, null, null, false);

                results.Count.Should().Be(1);
             
                var result1 = results.ElementAt(0);
                result1.CompanyName.Should().Be("My company");
                result1.SchemeName.Should().Be("Direct registrant");
                result1.TradingName.Should().BeNullOrWhiteSpace();
                result1.ProducerType.Should().Be("Registered company");
                result1.ProducerName.Should().Be("My company");
                result1.PRN.Should().Be("WEE/AG43365JN");
                result1.SICCodes.Should().BeNull();
                result1.VATRegistered.Should().BeNull();
                result1.AnnualTurnover.Should().BeNull();
                result1.AnnualTurnoverBandType.Should().BeNull();
                result1.EEEPlacedOnMarketBandType.Should().Be("Less than 5T EEE placed on market");
                result1.ObligationType.Should().Be("B2C");
                result1.ChargeBandType.Should().BeNull();
                result1.SellingTechniqueType.Should().Be("Direct Selling to End User");
                result1.CeaseToExist.Should().BeNull();
                result1.CNTitle.Should().BeNull();
                result1.CNForename.Should().BeNull();
                result1.CNSurname.Should().BeNull();
                result1.CNTelephone.Should().BeNull();
                result1.CNMobile.Should().BeNull();
                result1.CNFax.Should().BeNull();
                result1.CNEmail.Should().BeNull();
                result1.CNPrimaryName.Should().BeNull();
                result1.CNSecondaryName.Should().BeNull();
                result1.CNStreet.Should().BeNull();
                result1.CNTown.Should().BeNull();
                result1.CNLocality.Should().BeNull();
                result1.CNAdministrativeArea.Should().BeNull();
                result1.CNPostcode.Should().BeNull();
                result1.CNCountry.Should().BeNull();
                result1.CNCountry.Should().BeNull();
                result1.CompanyNumber.Should().Be("123456789");
                result1.CompanyContactTitle.Should().BeNull();
                result1.CompanyContactForename.Should().Be("first name");
                result1.CompanyContactSurname.Should().Be("last name");
                result1.CompanyContactTelephone.Should().Be("12345678");
                result1.CompanyContactMobile.Should().BeNull();
                result1.CompanyContactFax.Should().BeNull();
                result1.CompanyContactCountry.Should().Be("Azerbaijan");
                result1.CompanyContactEmail.Should().Be("test@co.uk");
                result1.CompanyContactPrimaryName.Should().Be("primary 1");
                result1.CompanyContactSecondaryName.Should().BeNull();
                result1.CompanyContactStreet.Should().Be("street");
                result1.CompanyContactTown.Should().Be("Woking");
                result1.CompanyContactLocality.Should().Be("Hampshire");
                result1.CompanyContactAdministrativeArea.Should().BeNull();
                result1.CompanyContactPostcode.Should().Be("GU21 5EE");
                result1.CompanyContactCountry.Should().Be("Azerbaijan");
                result1.PPOBContactTitle.Should().BeNull();
                result1.PPOBContactForename.Should().BeNull();
                result1.PPOBContactSurname.Should().BeNull();
                result1.PPOBContactTelephone.Should().BeNull();
                result1.PPOBContactMobile.Should().BeNull();
                result1.PPOBContactFax.Should().BeNull();
                result1.PPOBContactEmail.Should().BeNull();
                result1.PPOBContactPrimaryName.Should().BeNull();
                result1.PPOBContactSecondaryName.Should().BeNull();
                result1.PPOBContactStreet.Should().BeNull();
                result1.PPOBContactTown.Should().BeNull();
                result1.PPOBContactLocality.Should().BeNull();
                result1.PPOBContactAdministrativeArea.Should().BeNull();
                result1.PPOBContactPostcode.Should().BeNull();
                result1.OverseasProducerName.Should().BeNull();
                result1.OverseasContactForename.Should().BeNull();
                result1.OverseasContactSurname.Should().BeNull();
                result1.OverseasContactTelephone.Should().BeNull();
                result1.OverseasContactMobile.Should().BeNull();
                result1.OverseasContactFax.Should().BeNull();
                result1.OverseasContactEmail.Should().BeNull();
                result1.OverseasContactPrimaryName.Should().BeNull();
                result1.OverseasContactSecondaryName.Should().BeNull();
                result1.OverseasContactStreet.Should().BeNull();
                result1.OverseasContactTown.Should().BeNull();
                result1.OverseasContactLocality.Should().BeNull();
                result1.OverseasContactAdministrativeArea.Should().BeNull();
                result1.OverseasContactPostcode.Should().BeNull();
                result1.OverseasContactCountry.Should().BeNull();
                result1.RemovedFromScheme.Should().Be("No");
                result1.DateAmended.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromMinutes(2));
                result1.DateRegistered.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromMinutes(2));
                result1.BrandNames.Should().BeNullOrWhiteSpace();
            }
        }
    }
}
