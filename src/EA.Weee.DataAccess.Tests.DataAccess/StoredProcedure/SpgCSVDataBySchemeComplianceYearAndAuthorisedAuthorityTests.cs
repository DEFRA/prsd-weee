namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Domain.Producer.Classification;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthorityTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsProducerWithSelectedSchemeAndAA()
        {
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                scheme1.CompetentAuthorityId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                var results =
                    await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false,
                        false, scheme1.Id, scheme1.CompetentAuthorityId, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                var result = results[0];

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = false;

                helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                var result = results[0];

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                var result = results[0];

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsSoleTrader(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                var result = results[0];

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                var memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                var producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);
                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                var result = results[0];

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                var scheme2 = helper.CreateScheme();

                var memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                var producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/22BBBB22");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, null, null, false);

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";

                var producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";
                producer1.RegisteredProducer.Removed = true;

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";
                producer2.RegisteredProducer.Removed = true;

                var producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, true, false, scheme1.Id, null, false);

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                var producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";
                producer1.RegisteredProducer.Removed = true;

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";
                producer2.RegisteredProducer.Removed = true;

                var producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";
                producer3.RegisteredProducer.Removed = false;

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme1.Id, null, false);

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;
                memberUpload.SubmittedDate = new DateTime(2015, 1, 1);

                var producer = helper.CreateProducerAsPartnership(memberUpload, "WEE/11BBBB11");
                producer.Business.Partnership.Name = "ABCH";
                producer.RegisteredProducer.Removed = false;

                helper.CreateBrandName(producer, "Brand1");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, false, scheme.Id, null, false);

                // Assert
                Assert.NotNull(results);
                Assert.Single(results);

                Assert.Null(results[0].BrandNames);
            }
        }

        [Fact]
        public async Task Execute_WithProducerBrandNames_ReturnsBrandNames()
        {
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;
                memberUpload.SubmittedDate = new DateTime(2015, 1, 1);

                var producer = helper.CreateProducerAsPartnership(memberUpload, "WEE/11BBBB11");
                producer.Business.Partnership.Name = "ABCH";
                producer.RegisteredProducer.Removed = false;

                helper.CreateBrandName(producer, "Brand1");
                helper.CreateBrandName(producer, "Brand2");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, true, scheme.Id, null, false);

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
            using (var db = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(db.Model);

                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;
                memberUpload.SubmittedDate = new DateTime(2015, 1, 1);

                var producer = helper.CreateProducerAsPartnership(memberUpload, "WEE/11BBBB11");
                producer.Business.Partnership.Name = "ABCH";
                producer.RegisteredProducer.Removed = true;

                helper.CreateBrandName(producer, "Brand1");
                helper.CreateBrandName(producer, "Brand2");

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, true, true, scheme.Id, null, false);

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

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, amounts1, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                var paidDate = new DateTime(2020, 1, 1);
                var payment = await DirectRegistrantHelper.CreatePaymentSession(wrapper, submission, paidDate);

                submission.FinalPaymentSession = payment;
                submission.PaymentFinished = true;

                var authorisedRep = new Domain.Producer.AuthorisedRepresentative("authed rep name",
                    new ProducerContact("rep title", "rep first name", "rep surname",
                        "rep tel", "rep mob", "rep fax", "rep email", new ProducerAddress("rep address1",
                            "rep secondary", "rep street",
                            "rep town", "rep locality", "rep admin area", country, "rep postcode")));

                var brandNames = new BrandName("brand name");
                var (_, directRegistrant2, registeredProducer2) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 2", "WEE/AG48365JX", complianceYear, "987654321", authorisedRep, brandNames);

                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 4.456m, ObligationType = Domain.Obligation.ObligationType.B2B }
                };

                // not paid so should have null registered date
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

                var results = await wrapper.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(complianceYear, true, true, null, null, filterDirectRegistrant);

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
                result1.CompanyName.Should().Be("My company");
                result1.SchemeName.Should().Be("Direct registrant");
                result1.TradingName.Should().BeNullOrWhiteSpace();
                result1.ProducerType.Should().Be(EnumHelper.GetDisplayName(OrganisationType.RegisteredCompany));
                result1.ProducerName.Should().Be("My company");
                result1.PRN.Should().Be("WEE/AG48365JN");
                result1.SICCodes.Should().BeNull();
                result1.VATRegistered.Should().BeNull();
                result1.AnnualTurnover.Should().BeNull();
                result1.AnnualTurnoverBandType.Should().BeNull();
                result1.EEEPlacedOnMarketBandType.Should().Be(EnumHelper.GetDisplayName(EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket));
                result1.ObligationType.Should().Be(EnumHelper.GetDisplayName(ObligationType.B2C));
                result1.ChargeBandType.Should().BeNull();
                result1.SellingTechniqueType.Should().Be(EnumHelper.GetDescription(SellingTechniqueType.Both));
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
                result1.DateRegistered.Should().Be(paidDate);
                result1.BrandNames.Should().BeNullOrWhiteSpace();

                var result2 = results.ElementAt(assertionSmallProducerStartIndex + 1);
                result2.CompanyName.Should().Be("My company 2");
                result2.SchemeName.Should().Be("Direct registrant");
                result2.TradingName.Should().BeNullOrWhiteSpace();
                result2.ProducerType.Should().Be(EnumHelper.GetDisplayName(OrganisationType.RegisteredCompany));
                result2.ProducerName.Should().Be("My company 2");
                result2.PRN.Should().Be("WEE/AG48365JX");
                result2.SICCodes.Should().BeNull();
                result2.VATRegistered.Should().BeNull();
                result2.AnnualTurnover.Should().BeNull();
                result2.AnnualTurnoverBandType.Should().BeNull();
                result2.EEEPlacedOnMarketBandType.Should().Be(EnumHelper.GetDisplayName(EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket));
                result2.ObligationType.Should().Be(EnumHelper.GetDisplayName(ObligationType.B2B));
                result2.ChargeBandType.Should().BeNull();
                result2.SellingTechniqueType.Should().Be(EnumHelper.GetDescription(SellingTechniqueType.IndirectSellingtoEndUser));
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
                result2.OverseasProducerName.Should().Be("authed rep name");
                result2.OverseasContactForename.Should().Be("rep first name");
                result2.OverseasContactSurname.Should().Be("rep surname");
                result2.OverseasContactTelephone.Should().Be("rep tel");
                result2.OverseasContactMobile.Should().Be("rep mob");
                result2.OverseasContactFax.Should().Be("rep fax");
                result2.OverseasContactEmail.Should().Be("rep email");
                result2.OverseasContactPrimaryName.Should().Be("rep address1");
                result2.OverseasContactSecondaryName.Should().Be("rep secondary");
                result2.OverseasContactStreet.Should().Be("rep street");
                result2.OverseasContactTown.Should().Be("rep town");
                result2.OverseasContactLocality.Should().Be("rep locality");
                result2.OverseasContactAdministrativeArea.Should().Be("rep admin area");
                result2.OverseasContactPostcode.Should().Be("rep postcode");
                result2.OverseasContactCountry.Should().Be("Azerbaijan");
                result2.RemovedFromScheme.Should().Be("Yes");
                result2.DateAmended.Should().BeCloseTo(SystemTime.UtcNow, TimeSpan.FromMinutes(2));
                result2.DateRegistered.Should().BeNull();
                result2.BrandNames.Should().Be("brand name");
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantAndSchemeSubmissions_GivenOnlySchemeFilter_ShouldOnlyReturnSchemeResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                var complianceYear = 2056;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                // create the submission that should not be returned
                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, amounts1, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                await wrapper.WeeeContext.SaveChangesAsync();

                // Create a scheme that should be returned
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

                var results = await wrapper.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(complianceYear, true, true, null, null, false, true);

                results.Count.Should().Be(1);

                var schemeResult = results.ElementAt(0);
                schemeResult.ApprovalNumber.Should().Be("WEE/AH7453NF/SCH");
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantAndSchemeSubmissions_GivenOnlySchemeId_ShouldOnlyReturnSchemeResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                var complianceYear = 2053;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JX", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                // create the submission that should not be returned
                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, amounts1, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                await wrapper.WeeeContext.SaveChangesAsync();

                // Create a scheme that should be returned
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
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH7453ZF/SCH", "WEE9462846",
                    Domain.Obligation.ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                var schemeRegisteredProducer1 =
                    new Domain.Producer.RegisteredProducer("WEE/AG48165JE", complianceYear, scheme1);

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

                var results = await wrapper.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(complianceYear, true, true, scheme1.Id, null, false, false);

                results.Count.Should().Be(1);

                var schemeResult = results.ElementAt(0);
                schemeResult.ApprovalNumber.Should().Be("WEE/AH7453ZF/SCH");
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_WithExcludeRemovedProducer_ShouldReturnNoResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

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
        public async Task Execute_WithDirectRegistrantSubmissions_WithReturnedSubmissionThatHasBeenReSubmitted_ShouldUseRecentSubmittedData()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2082;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG43365JN", complianceYear);

                // initially no EEE and selling technique type of both
                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                var paidDate = new DateTime(2020, 1, 1);
                var payment = await DirectRegistrantHelper.CreatePaymentSession(wrapper, submission1, paidDate);

                submission1.FinalPaymentSession = payment;
                submission1.PaymentFinished = true;

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
                result1.ProducerType.Should().Be(EnumHelper.GetDisplayName(OrganisationType.RegisteredCompany));
                result1.ProducerName.Should().Be("My company");
                result1.PRN.Should().Be("WEE/AG43365JN");
                result1.SICCodes.Should().BeNull();
                result1.VATRegistered.Should().BeNull();
                result1.AnnualTurnover.Should().BeNull();
                result1.AnnualTurnoverBandType.Should().BeNull();
                result1.EEEPlacedOnMarketBandType.Should().Be(EnumHelper.GetDisplayName(EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket));
                result1.ObligationType.Should().Be(EnumHelper.GetDisplayName(ObligationType.B2C));
                result1.ChargeBandType.Should().BeNull();
                result1.SellingTechniqueType.Should().Be(EnumHelper.GetDescription(SellingTechniqueType.DirectSellingtoEndUser));
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
                result1.DateRegistered.Should().Be(paidDate);
                result1.BrandNames.Should().BeNullOrWhiteSpace();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_WithReturnedSubmissionThatHasNotBeenReSubmitted_ShouldUseRecentSubmittedData()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2082;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG13365JN", complianceYear);

                // initially no EEE and selling technique type of both
                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                // return the submission update the amounts but is not submitted, should use original empty values
                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var paidDate = new DateTime(2023, 1, 1);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission1, paidDate);

                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission1);

                await DirectRegistrantHelper.UpdateEeeeAmounts(wrapper, submission1, amounts1);

                await wrapper.WeeeContext.SaveChangesAsync();

                var results = await wrapper.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(complianceYear, true, true, null, null, false);

                results.Count.Should().Be(1);

                var result1 = results.ElementAt(0);
                result1.CompanyName.Should().Be("My company");
                result1.SchemeName.Should().Be("Direct registrant");
                result1.TradingName.Should().BeNullOrWhiteSpace();
                result1.ProducerType.Should().Be(EnumHelper.GetDisplayName(OrganisationType.RegisteredCompany));
                result1.ProducerName.Should().Be("My company");
                result1.PRN.Should().Be("WEE/AG13365JN");
                result1.SICCodes.Should().BeNull();
                result1.VATRegistered.Should().BeNull();
                result1.AnnualTurnover.Should().BeNull();
                result1.AnnualTurnoverBandType.Should().BeNull();
                result1.EEEPlacedOnMarketBandType.Should().Be(EnumHelper.GetDisplayName(EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket));
                result1.ObligationType.Should().Be("Unknown");
                result1.ChargeBandType.Should().BeNull();
                result1.SellingTechniqueType.Should().Be(EnumHelper.GetDescription(SellingTechniqueType.Both));
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
                result1.DateRegistered.Should().Be(paidDate);
                result1.BrandNames.Should().BeNullOrWhiteSpace();
            }
        }
        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_WithReturnedSubmissionThatHasNotBeenReSubmitte_UsingOMP_ShouldUseRecentSubmittedData()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                var complianceYear = DateTime.Now.Year;
                const string prn = "WEE/AG53365JN";
                const string companyName = "My company";
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, companyName, prn, complianceYear);

                // initially no EEE and selling technique type of both
                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.OnlineMarketplace.Value);

                // return the submission update the amounts but is not submitted, should use original empty values
                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var paidDate = new DateTime(2023, 1, 1);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission1, paidDate);

                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission1);

                await DirectRegistrantHelper.UpdateEeeeAmounts(wrapper, submission1, amounts1);

                await wrapper.WeeeContext.SaveChangesAsync();

                var results = await wrapper.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(complianceYear, true, true, null, null, false);

                results.Count.Should().Be(1);

                var result1 = results.ElementAt(0);
                result1.CompanyName.Should().Be(companyName);
                result1.SchemeName.Should().Be("Direct registrant");
                result1.TradingName.Should().BeNullOrWhiteSpace();
                result1.ProducerType.Should().Be(EnumHelper.GetDisplayName(OrganisationType.RegisteredCompany));
                result1.ProducerName.Should().Be(companyName);
                result1.PRN.Should().Be(prn);
                result1.SICCodes.Should().BeNull();
                result1.VATRegistered.Should().BeNull();
                result1.AnnualTurnover.Should().BeNull();
                result1.AnnualTurnoverBandType.Should().BeNull();
                result1.EEEPlacedOnMarketBandType.Should().Be(EnumHelper.GetDisplayName(EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket));
                result1.ObligationType.Should().Be("Unknown");
                result1.ChargeBandType.Should().BeNull();
                result1.SellingTechniqueType.Should().Be(EnumHelper.GetDescription(SellingTechniqueType.OnlineMarketplace));
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
                result1.DateRegistered.Should().Be(paidDate);
                result1.BrandNames.Should().BeNullOrWhiteSpace();
            }
        }
    }
}
