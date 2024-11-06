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

    public class SpgUKEEEDataByComplianceYearTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsUkEeeDataWithSelectedComplianceYear()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/TE0000ST/SCH";
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2000;

                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2001;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "PRN123");
                var producer2 = helper.CreateProducerAsCompany(memberUpload, "PRN456");

                var producer3 = helper.CreateProducerAsCompany(memberUpload1, "PRN777");

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2000, 2);

                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme, 20001, 1);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2C", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, "B2C", 2, 1000);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer2.RegisteredProducer, "B2B", 2, 400);

                helper.CreateEeeOutputAmount(dataReturnVersion3, producer3.RegisteredProducer, "B2C", 2, 8000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgUKEEEDataByComplianceYear(2000);

                //Assert
                Assert.NotNull(results);

                Assert.Equal(14, results.Count);

                var firstCategoryRecord = results[0];
                var secondCategoryRecord = results[1];

                Assert.Equal("01. Large household appliances", firstCategoryRecord.Category);
                Assert.Equal("02. Small household appliances", secondCategoryRecord.Category);

                Assert.Equal(400, secondCategoryRecord.Q1B2BEEE);
                Assert.Equal(100, firstCategoryRecord.Q1B2CEEE);
                Assert.Equal(1000, secondCategoryRecord.Q2B2CEEE);

                Assert.Equal(0, firstCategoryRecord.TotalB2BEEE);
                Assert.Equal(400, secondCategoryRecord.TotalB2BEEE);
                Assert.Equal(100, firstCategoryRecord.TotalB2CEEE);
                Assert.Equal(1000, secondCategoryRecord.TotalB2CEEE);
            }
        }

        [Fact]
        public async Task Execute_ReturnsUkEeeData_ForNonRemovedProducers()
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
                var producer3 = helper.CreateProducerAsCompany(memberUpload, "PRN777");
                producer3.RegisteredProducer.Removed = true;

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2000, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2000, 2);
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme, 2000, 3);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme, 2000, 4);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2C", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer2.RegisteredProducer, "B2C", 1, 200);
                helper.CreateEeeOutputAmount(dataReturnVersion3, producer3.RegisteredProducer, "B2C", 1, 300);
                helper.CreateEeeOutputAmount(dataReturnVersion4, producer2.RegisteredProducer, "B2C", 1, 400);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgUKEEEDataByComplianceYear(2000);

                //Assert
                Assert.NotNull(results);
                Assert.Equal(14, results.Count);
                var firstCategoryRecord = results[0];
                Assert.Equal("01. Large household appliances", firstCategoryRecord.Category);

                Assert.Null(firstCategoryRecord.Q3B2CEEE);
                Assert.Equal(700, firstCategoryRecord.TotalB2CEEE);
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_ReturnsResults()
        {
            // Arrange
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                var complianceYear = 2060;
                // Direct registrant data is for the previous year
                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear - 1);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 456.789m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 222.111m, ObligationType = Domain.Obligation.ObligationType.B2B }, // Should be excluded as B2B
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

                // Act
                var results = await wrapper.WeeeContext.StoredProcedures.SpgUKEEEDataByComplianceYear(complianceYear);

                results.Should().NotBeNull();
                results.Count.Should().Be(14);

                var schemeElement = results.ElementAt(0);
                schemeElement.Category.Should().Be("01. Large household appliances");
                schemeElement.Q1B2CEEE.Should().Be(123.457m);
                schemeElement.Q4B2BEEE.Should().Be(222.111m);
                schemeElement.TotalB2BEEE.Should().Be(222.111m);
                schemeElement.Q4B2CEEE.Should().Be(456.789m);
                schemeElement.TotalB2CEEE.Should().Be(580.246m);
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_WithReturnedSubmissionThatHasNotBeenReSubmitted_ReturnsResults()
        {
            // Arrange
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, _) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                var complianceYear = 2037;
                // Direct registrant data is for the previous year
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG43365JN", complianceYear - 1);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 456.789m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 222.111m, ObligationType = Domain.Obligation.ObligationType.B2B }, 
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, amounts1, DirectProducerSubmissionStatus.Complete);

                // These amounts should not be considered as they have not been re-submitted
                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ITAndTelecommsEquipment, Amount = 1m, ObligationType = Domain.Obligation.ObligationType.B2B },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2B },
                };

                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission1, amounts2);

                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                var results = await wrapper.WeeeContext.StoredProcedures.SpgUKEEEDataByComplianceYear(complianceYear);

                results.Should().NotBeNull()
                    .And.HaveCount(14);

                var categoryExpectations = new Dictionary<string, object>
                {
                    ["01. Large household appliances"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = 222.111M,
                        Q4B2CEEE = 456.789M,
                        TotalB2BEEE = 222.111M,
                        TotalB2CEEE = 456.789M
                    },
                    ["02. Small household appliances"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["03. IT and telecommunications equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["04. Consumer equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = 2M,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 2M
                    },
                    ["05. Lighting equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["06. Electrical and electronic tools"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["07. Toys, leisure and sports equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["08. Medical devices"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["09. Monitoring and control instruments"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["10. Automatic dispensers"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["11. Display equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["12. Appliances containing refrigerants"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["13. Gas discharge lamps and LED light sources"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["14. Photovoltaic panels"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    }
                };

                foreach (var categoryEntry in categoryExpectations)
                {
                    var category = results.Should().ContainSingle(r => r.Category == categoryEntry.Key)
                        .Subject;

                    category.Should().BeEquivalentTo(categoryEntry.Value,
                        options => options.WithStrictOrdering()
                            .ComparingByMembers<object>());
                }
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissions_WithReturnedSubmissionThatHasBeenReSubmitted_ReturnsResults()
        {
            // Arrange
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, _) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                var complianceYear = 2036;
                // Direct registrant data is for the previous year
                var (_, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG43345JN", complianceYear - 1);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 456.789m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 222.111m, ObligationType = Domain.Obligation.ObligationType.B2B },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission1 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, amounts1, DirectProducerSubmissionStatus.Complete);

                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission1);

                // These amounts should be considered as they have been re-submitted
                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ITAndTelecommsEquipment, Amount = 1m, ObligationType = Domain.Obligation.ObligationType.B2B },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2B },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MonitoringAndControlInstruments, Amount = 3m, ObligationType = Domain.Obligation.ObligationType.B2C },
                };

                await DirectRegistrantHelper.SubmitSubmission(wrapper, submission1, amounts2);

                await wrapper.WeeeContext.SaveChangesAsync();

                // Act
                var results = await wrapper.WeeeContext.StoredProcedures.SpgUKEEEDataByComplianceYear(complianceYear);

                results.Should().NotBeNull()
                    .And.HaveCount(14);

                var categoryExpectations = new Dictionary<string, object>
                {
                    ["01. Large household appliances"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["02. Small household appliances"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["03. IT and telecommunications equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = 1M,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 1M,
                        TotalB2CEEE = 0M
                    },
                    ["04. Consumer equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["05. Lighting equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["06. Electrical and electronic tools"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["07. Toys, leisure and sports equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["08. Medical devices"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = 2M,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 2M,
                        TotalB2CEEE = 0M
                    },
                    ["09. Monitoring and control instruments"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = 3M,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 3M
                    },
                    ["10. Automatic dispensers"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["11. Display equipment"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["12. Appliances containing refrigerants"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["13. Gas discharge lamps and LED light sources"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    },
                    ["14. Photovoltaic panels"] = new
                    {
                        Q1B2CEEE = (decimal?)null,
                        Q1B2BEEE = (decimal?)null,
                        Q2B2BEEE = (decimal?)null,
                        Q2B2CEEE = (decimal?)null,
                        Q3B2BEEE = (decimal?)null,
                        Q3B2CEEE = (decimal?)null,
                        Q4B2BEEE = (decimal?)null,
                        Q4B2CEEE = (decimal?)null,
                        TotalB2BEEE = 0M,
                        TotalB2CEEE = 0M
                    }
                };

                foreach (var categoryEntry in categoryExpectations)
                {
                    var category = results.Should().ContainSingle(r => r.Category == categoryEntry.Key)
                        .Subject;

                    category.Should().BeEquivalentTo(categoryEntry.Value,
                        options => options.WithStrictOrdering()
                            .ComparingByMembers<object>());
                }
            }
        }
    }
}
