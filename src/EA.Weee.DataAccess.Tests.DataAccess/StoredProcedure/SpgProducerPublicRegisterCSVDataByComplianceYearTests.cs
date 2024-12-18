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

    public class SpgProducerPublicRegisterCSVDataByComplianceYearTests
    {
        [Fact]
        public async Task Execute_HappyPath_ProducerTypeIsCompany_PerfectAssociationOfSchemeToOrganisationAndReturnsSelectedComplianceYearRecords()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.SchemeName = "SchemeName";

                scheme1.Organisation.Name = "PCS operator name";
                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");
                producerSubmission1.Business.Company.Contact1.Telephone = "55 5555 5555";
                producerSubmission1.Business.Company.Contact1.Email = "email@test.com";

                Scheme scheme2 = helper.CreateScheme();

                scheme2.OrganisationId = new Guid("84729d95-3c26-4c37-9730-f25fa2b6dbfc");
                scheme2.Organisation.Id = new Guid("84729d95-3c26-4c37-9730-f25fa2b6dbfc");
                scheme2.SchemeName = "A test scheme name";

                scheme2.Organisation.Name = "PCS operator name 1";
                scheme2.Organisation.BusinessAddressId = new Guid("ecdf60fc-1576-43e2-9bb4-e6c9dc9d721d");
                scheme2.Organisation.Address = helper.CreateOrganisationAddress();
                scheme2.Organisation.Address.Id = new Guid("ecdf60fc-1576-43e2-9bb4-e6c9dc9d721d");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, "WEE/11ZZZZ11");
                producerSubmission2.Business.Company.Contact1.Telephone = "55 5555 5555";
                producerSubmission2.Business.Company.Contact1.Email = "email@test.com";

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                    await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.TrueForAll(i => i.ComplianceYear == 2016));

                Assert.DoesNotContain(results, i => i.PRN == "WEE/11ZZZZ11" && i.ComplianceYear == 2017);

                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99" && i.ComplianceYear == 2016);

                Assert.NotNull(result);
                Assert.Equal(result.CSROAAddress1, scheme1.Organisation.Address.Address1);
                Assert.Equal(result.CSROAPostcode, scheme1.Organisation.Address.Postcode);

                Assert.Equal(result.SchemeName, scheme1.SchemeName);
                Assert.Equal(result.SchemeOperator, scheme1.Organisation.Name);

                Assert.Equal(result.ROATelephone, producerSubmission1.Business.Company.Contact1.Telephone);
                Assert.Equal(result.ROAEmail, producerSubmission1.Business.Company.Contact1.Email);
            }
        }

        [Fact]
        public async Task Execute_NonSubmittedMemberUpload_IgnoresProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");

                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = false;

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                   await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                Assert.DoesNotContain(results, i => i.PRN == "WEE/99ZZZZ99");
            }
        }

        [Fact]
        public async Task Execute_ProducerTypeIsCompany_ReturnsCompanyNameAsProducerName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");

                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                   await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99");

                Assert.NotNull(result);
                Assert.Equal(result.ProducerName, producerSubmission.Business.Company.Name);
            }
        }

        [Fact]
        public async Task Execute_ProducerTypeIsPartnership_ReturnsPartnershipNameAsProducerName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");

                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission = helper.CreateProducerAsPartnership(memberUpload1, "WEE/99ZZZZ99");

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                   await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99");

                Assert.NotNull(result);
                Assert.Equal(result.ProducerName, producerSubmission.Business.Partnership.Name);
            }
        }

        [Fact]
        public async Task Execute_ListOfProducers_ReturnsRecordsInCurrectOrder()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.SchemeName = "Lamborghini";

                scheme1.Organisation.Name = "PCS operator name";
                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");
                producerSubmission1.Business.Company.Name = "A company name";

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/88ZZZZ88");
                producerSubmission2.Business.Company.Name = "B company name";

                Scheme scheme2 = helper.CreateScheme();

                scheme2.OrganisationId = new Guid("84729d95-3c26-4c37-9730-f25fa2b6dbfc");
                scheme2.Organisation.Id = new Guid("84729d95-3c26-4c37-9730-f25fa2b6dbfc");
                scheme2.SchemeName = "Aston Martin";

                scheme2.Organisation.Name = "PCS operator name 1";
                scheme2.Organisation.BusinessAddressId = new Guid("ecdf60fc-1576-43e2-9bb4-e6c9dc9d721d");
                scheme2.Organisation.Address = helper.CreateOrganisationAddress();
                scheme2.Organisation.Address.Id = new Guid("ecdf60fc-1576-43e2-9bb4-e6c9dc9d721d");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission3 = helper.CreateProducerAsCompany(memberUpload2, "WEE/11ZZZZ11");
                producerSubmission3.Business.Company.Name = "C company name";

                Weee.Tests.Core.Model.ProducerSubmission producerSubmission4 = helper.CreateProducerAsCompany(memberUpload2, "WEE/22ZZZZ22");
                producerSubmission4.Business.Company.Name = "D company name";

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                    await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.FindIndex(i => i.SchemeName == "Aston Martin" && i.ProducerName == "C company name") < results.FindIndex(i => i.SchemeName == "Aston Martin" && i.ProducerName == "D company name"));
                Assert.True(results.FindIndex(i => i.SchemeName == "Lamborghini" && i.ProducerName == "A company name") < results.FindIndex(i => i.SchemeName == "Lamborghini" && i.ProducerName == "B company name"));
                Assert.True(results.FindIndex(i => i.SchemeName == "Aston Martin" && i.ProducerName == "C company name") < results.FindIndex(i => i.SchemeName == "Lamborghini" && i.ProducerName == "B company name"));
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_ReturnsResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2060;
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
                var (_, directRegistrant2, registeredProducer2) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 2", "WEE/AG48365JX", complianceYear, "987654321", authorisedRep, brandNames, "My company 2 trading");

                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 4.456m, ObligationType = Domain.Obligation.ObligationType.B2B },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 4.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant2, registeredProducer2, complianceYear, amounts2, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.IndirectSellingtoEndUser.Value);

                // removed producer should not be included
                var (_, directRegistrant3, registeredProducer3) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 3", "WEE/AG18365JX", complianceYear, "987654321", authorisedRep, brandNames);

                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant3, registeredProducer3, complianceYear, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.IndirectSellingtoEndUser.Value);

                registeredProducer3.Remove();

                // Create a scheme for test or ordering
                var organisation =
                    Domain.Organisation.Organisation.CreateSoleTrader("Test Organisation");
                organisation.AddOrUpdateAddress(Domain.AddressType.RegisteredOrPPBAddress, new Domain.Organisation.Address("address1", "address2", "town", "county", "gu", country, "1234", "1@1.com"));

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

                var results = await wrapper.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(complianceYear);

                var schemeResult = results.ElementAt(0);
                schemeResult.SchemeName.Should().Be(scheme1.SchemeName);

                var result1 = results.ElementAt(1);
                result1.CompanyName.Should().Be("My company");
                result1.SchemeName.Should().Be("Direct registrant");
                result1.TradingName.Should().BeNullOrWhiteSpace();
                result1.ProducerName.Should().Be("My company");
                result1.PRN.Should().Be("WEE/AG48365JN");
                result1.ObligationType.Should().Be("B2C");
                result1.ROAPrimaryName.Should().Be("primary 1");
                result1.ROASecondaryName.Should().BeNull();
                result1.ROATown.Should().Be("Woking");
                result1.ROALocality.Should().Be("Hampshire");
                result1.ROAAdministrativeArea.Should().BeNull();
                result1.ROAPostCode.Should().Be("GU21 5EE");
                result1.ROACountry.Should().Be("Azerbaijan");
                result1.ROATelephone.Should().Be("12345678");
                result1.ROAEmail.Should().Be("test@co.uk");
                result1.CSROAAddress1.Should().BeNull();
                result1.CSROAAddress2.Should().BeNull();
                result1.CSROATownOrCity.Should().BeNull();
                result1.CSROATownOrCity.Should().BeNull();
                result1.CSROACountyOrRegion.Should().BeNull();
                result1.CSROAPostcode.Should().BeNull();
                result1.CSROACountry.Should().BeNull();
                result1.OPNAName.Should().BeNull();
                result1.OPNAPrimaryName.Should().BeNull();
                result1.OPNASecondaryName.Should().BeNull();
                result1.OPNAStreet.Should().BeNull();
                result1.OPNATown.Should().BeNull();
                result1.OPNALocality.Should().BeNull();
                result1.OPNAAdministrativeArea.Should().BeNull();
                result1.OPNACountry.Should().BeNull();
                result1.OPNAPostCode.Should().BeNull();
                result1.ComplianceYear.Should().Be(complianceYear);
                result1.PPOBPrimaryName.Should().BeNull();
                result1.PPOBSecondaryName.Should().BeNull();
                result1.PPOBStreet.Should().BeNull();
                result1.PPOBTown.Should().BeNull();
                result1.PPOBLocality.Should().BeNull();
                result1.PPOBAdministrativeArea.Should().BeNull();
                result1.PPOBCountry.Should().BeNull();
                result1.PPOBPostcode.Should().BeNull();

                var result2 = results.ElementAt(2);
                result2.CompanyName.Should().Be("My company 2");
                result2.SchemeName.Should().Be("Direct registrant");
                result2.TradingName.Should().Be("My company 2 trading");
                result2.ProducerName.Should().Be("My company 2");
                result2.PRN.Should().Be("WEE/AG48365JX");
                result2.ObligationType.Should().Be("Both");
                result2.ROAPrimaryName.Should().Be("primary 1");
                result2.ROASecondaryName.Should().BeNull();
                result2.ROATown.Should().Be("Woking");
                result2.ROALocality.Should().Be("Hampshire");
                result2.ROAAdministrativeArea.Should().BeNull();
                result2.ROAPostCode.Should().Be("GU21 5EE");
                result2.ROACountry.Should().Be("Azerbaijan");
                result2.ROATelephone.Should().Be("12345678");
                result2.ROAEmail.Should().Be("test@co.uk");
                result2.CSROAAddress1.Should().BeNull();
                result2.CSROAAddress2.Should().BeNull();
                result2.CSROATownOrCity.Should().BeNull();
                result2.CSROATownOrCity.Should().BeNull();
                result2.CSROACountyOrRegion.Should().BeNull();
                result2.CSROAPostcode.Should().BeNull();
                result2.CSROACountry.Should().BeNull();
                result2.OPNAName.Should().Be("authed rep name");
                result2.OPNAPrimaryName.Should().Be("rep address1");
                result2.OPNASecondaryName.Should().Be("rep secondary");
                result2.OPNAStreet.Should().Be("rep street");
                result2.OPNATown.Should().Be("rep town");
                result2.OPNALocality.Should().Be("rep locality");
                result2.OPNAAdministrativeArea.Should().Be("rep admin area");
                result2.OPNACountry.Should().Be("Azerbaijan");
                result2.OPNAPostCode.Should().Be("rep postcode");
                result2.ComplianceYear.Should().Be(complianceYear);
                result2.PPOBPrimaryName.Should().BeNull();
                result2.PPOBSecondaryName.Should().BeNull();
                result2.PPOBStreet.Should().BeNull();
                result2.PPOBTown.Should().BeNull();
                result2.PPOBLocality.Should().BeNull();
                result2.PPOBAdministrativeArea.Should().BeNull();
                result2.PPOBCountry.Should().BeNull();
                result2.PPOBPostcode.Should().BeNull();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsThatHaveBeenReturnedAndResubmitted_ShouldReturnMostRecentSubmittedData()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2063;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AZ48365JN", complianceYear);

                // first submission is B2C
                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 1m, ObligationType = Domain.Obligation.ObligationType.B2C },
                };

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, amounts1, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission);

                // re-submission submission is B2B
                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 1m, ObligationType = Domain.Obligation.ObligationType.B2B },
                };

                await DirectRegistrantHelper.SubmitSubmission(wrapper, submission, amounts2);

                var results = await wrapper.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(complianceYear);

                var result1 = results.ElementAt(0);
                result1.CompanyName.Should().Be("My company");
                result1.SchemeName.Should().Be("Direct registrant");
                result1.TradingName.Should().BeNullOrWhiteSpace();
                result1.ProducerName.Should().Be("My company");
                result1.PRN.Should().Be("WEE/AZ48365JN");
                result1.ObligationType.Should().Be("B2B");
                result1.ROAPrimaryName.Should().Be("primary 1");
                result1.ROASecondaryName.Should().BeNull();
                result1.ROATown.Should().Be("Woking");
                result1.ROALocality.Should().Be("Hampshire");
                result1.ROAAdministrativeArea.Should().BeNull();
                result1.ROAPostCode.Should().Be("GU21 5EE");
                result1.ROACountry.Should().Be("Azerbaijan");
                result1.ROATelephone.Should().Be("12345678");
                result1.ROAEmail.Should().Be("test@co.uk");
                result1.CSROAAddress1.Should().BeNull();
                result1.CSROAAddress2.Should().BeNull();
                result1.CSROATownOrCity.Should().BeNull();
                result1.CSROATownOrCity.Should().BeNull();
                result1.CSROACountyOrRegion.Should().BeNull();
                result1.CSROAPostcode.Should().BeNull();
                result1.CSROACountry.Should().BeNull();
                result1.OPNAName.Should().BeNull();
                result1.OPNAPrimaryName.Should().BeNull();
                result1.OPNASecondaryName.Should().BeNull();
                result1.OPNAStreet.Should().BeNull();
                result1.OPNATown.Should().BeNull();
                result1.OPNALocality.Should().BeNull();
                result1.OPNAAdministrativeArea.Should().BeNull();
                result1.OPNACountry.Should().BeNull();
                result1.OPNAPostCode.Should().BeNull();
                result1.ComplianceYear.Should().Be(complianceYear);
                result1.PPOBPrimaryName.Should().BeNull();
                result1.PPOBSecondaryName.Should().BeNull();
                result1.PPOBStreet.Should().BeNull();
                result1.PPOBTown.Should().BeNull();
                result1.PPOBLocality.Should().BeNull();
                result1.PPOBAdministrativeArea.Should().BeNull();
                result1.PPOBCountry.Should().BeNull();
                result1.PPOBPostcode.Should().BeNull();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsThatHaveBeenReturnedAndNotResubmitted_ShouldReturnMostRecentSubmittedData()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2045;
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AZ18365JN", complianceYear);

                // first submission is B2C
                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 1m, ObligationType = Domain.Obligation.ObligationType.B2C },
                };

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear, amounts1, DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                // re-submission submission is B2B and should use original submitted record
                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 1m, ObligationType = Domain.Obligation.ObligationType.B2B },
                };

                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission);

                await DirectRegistrantHelper.UpdateEeeeAmounts(wrapper, submission, amounts2);

                var results = await wrapper.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(complianceYear);

                var result1 = results.ElementAt(0);
                result1.CompanyName.Should().Be("My company");
                result1.SchemeName.Should().Be("Direct registrant");
                result1.TradingName.Should().BeNullOrWhiteSpace();
                result1.ProducerName.Should().Be("My company");
                result1.PRN.Should().Be("WEE/AZ18365JN");
                result1.ObligationType.Should().Be("B2C");
                result1.ROAPrimaryName.Should().Be("primary 1");
                result1.ROASecondaryName.Should().BeNull();
                result1.ROATown.Should().Be("Woking");
                result1.ROALocality.Should().Be("Hampshire");
                result1.ROAAdministrativeArea.Should().BeNull();
                result1.ROAPostCode.Should().Be("GU21 5EE");
                result1.ROACountry.Should().Be("Azerbaijan");
                result1.ROATelephone.Should().Be("12345678");
                result1.ROAEmail.Should().Be("test@co.uk");
                result1.CSROAAddress1.Should().BeNull();
                result1.CSROAAddress2.Should().BeNull();
                result1.CSROATownOrCity.Should().BeNull();
                result1.CSROATownOrCity.Should().BeNull();
                result1.CSROACountyOrRegion.Should().BeNull();
                result1.CSROAPostcode.Should().BeNull();
                result1.CSROACountry.Should().BeNull();
                result1.OPNAName.Should().BeNull();
                result1.OPNAPrimaryName.Should().BeNull();
                result1.OPNASecondaryName.Should().BeNull();
                result1.OPNAStreet.Should().BeNull();
                result1.OPNATown.Should().BeNull();
                result1.OPNALocality.Should().BeNull();
                result1.OPNAAdministrativeArea.Should().BeNull();
                result1.OPNACountry.Should().BeNull();
                result1.OPNAPostCode.Should().BeNull();
                result1.ComplianceYear.Should().Be(complianceYear);
                result1.PPOBPrimaryName.Should().BeNull();
                result1.PPOBSecondaryName.Should().BeNull();
                result1.PPOBStreet.Should().BeNull();
                result1.PPOBTown.Should().BeNull();
                result1.PPOBLocality.Should().BeNull();
                result1.PPOBAdministrativeArea.Should().BeNull();
                result1.PPOBCountry.Should().BeNull();
                result1.PPOBPostcode.Should().BeNull();
            }
        }
    }
}
