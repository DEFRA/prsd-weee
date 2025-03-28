namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Producer.Classification;
    using EA.Prsd.Core.Helpers;
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
                const int complianceYear = 2000;
                memberUpload.ComplianceYear = complianceYear;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "PRN123");
                var producer2 = helper.CreateProducerAsCompany(memberUpload, "PRN456");

                var quarter1Year1DataReturnVersion = helper.CreateDataReturnVersion(scheme, complianceYear, 1);
                var quarter2Year1DataReturnVersion = helper.CreateDataReturnVersion(scheme, complianceYear, 2);

                var quarter1PreviousYearDataReturnVersion = helper.CreateDataReturnVersion(scheme, complianceYear - 1, 1);
                var quarter1FollowingYearDataReturnVersion = helper.CreateDataReturnVersion(scheme, complianceYear + 1, 1);

                const string b2cObligationType = "B2C";
                const string b2bObligationType = "B2B";

                const int weeeCategory1 = 1;
                const int weeeCategory2 = 2;

                // data we should retrieve
                const int quarter1Category1Tonnage = 100;
                helper.CreateEeeOutputAmount(quarter1Year1DataReturnVersion, producer1.RegisteredProducer, b2cObligationType, weeeCategory1, quarter1Category1Tonnage);
                const int quarter2Category2Tonnage = 1000;
                helper.CreateEeeOutputAmount(quarter2Year1DataReturnVersion, producer1.RegisteredProducer, b2cObligationType, weeeCategory2, quarter2Category2Tonnage);

                // data we should not retrieve
                // same year, different obligation type (and different producer)
                helper.CreateEeeOutputAmount(quarter1Year1DataReturnVersion, producer2.RegisteredProducer, b2bObligationType, weeeCategory2, 400);
                // same year, different obligation type, same producer
                helper.CreateEeeOutputAmount(quarter1Year1DataReturnVersion, producer1.RegisteredProducer, b2bObligationType, weeeCategory2, 400);

                // earlier year, same obligation type
                helper.CreateEeeOutputAmount(quarter1PreviousYearDataReturnVersion, producer1.RegisteredProducer, b2cObligationType, weeeCategory2, 400);

                // later year, same obligation type
                helper.CreateEeeOutputAmount(quarter1FollowingYearDataReturnVersion, producer1.RegisteredProducer, b2cObligationType, weeeCategory2, 400);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, b2cObligationType, false, false);

                //Assert
                Assert.NotNull(results);

                //Data return with obliation type B2B should not be there in the result.
                ProducerEeeCsvData b2bProducer = results.Find(x => (x.PRN == "PRN456"));
                Assert.Null(b2bProducer);

                ProducerEeeCsvData result = results[0];
                Assert.Equal("test scheme name", result.SchemeName);
                Assert.Equal("WEE/TE0000ST/SCH", result.ApprovalNumber);
                Assert.Equal("PRN123", result.PRN);
                Assert.Equal(quarter1Category1Tonnage, result.Cat1Q1);
                Assert.Equal(quarter2Category2Tonnage, result.Cat2Q2);
                Assert.Equal(quarter1Category1Tonnage + quarter2Category2Tonnage, result.TotalTonnage);
                Assert.Null(result.Cat1Q3);
            }
        }

        [Fact]
        public async Task Execute_HappyPath_ReturnsProducerEeeWithSelectedYearAndObligationType_Where_ProducerDataHasBoth()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                string approvalNumber = "WEE/TE0000ST/SCH";
                scheme.ApprovalNumber = approvalNumber;
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                int complianceYear = DateTime.Now.Year;
                memberUpload.ComplianceYear = complianceYear;
                string prn = "PRN123";

                string b2bObligationType = EnumHelper.GetDisplayName(ObligationType.B2B);
                string b2cObligationType = EnumHelper.GetDisplayName(ObligationType.B2C);

                var producer = helper.CreateProducerAsCompany(memberUpload, prn);

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, complianceYear, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, complianceYear, 2);
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme, complianceYear, 3);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme, complianceYear, 4);

                var categories = EnumHelper.GetValues(typeof(WeeeCategory));
                var maxCategoryId = categories.Max(x => x.Key);
                int b2bQuarter1Tonnage = 0;
                int b2bQuarter2Tonnage = 0;
                int b2bQuarter3Tonnage = 0;
                int b2bQuarter4Tonnage = 0;

                int b2cQuarter1Tonnage = 0;
                int b2cQuarter2Tonnage = 0;
                int b2cQuarter3Tonnage = 0;
                int b2cQuarter4Tonnage = 0;

                for (int categoryId = 1; categoryId <= maxCategoryId; categoryId++)
                {
                    b2bQuarter1Tonnage = b2bQuarter1Tonnage + 1;
                    b2bQuarter2Tonnage = b2bQuarter2Tonnage + 2;
                    b2bQuarter3Tonnage = b2bQuarter3Tonnage + 3;
                    b2bQuarter4Tonnage = b2bQuarter4Tonnage + 4;

                    b2cQuarter1Tonnage = b2cQuarter1Tonnage + 5;
                    b2cQuarter2Tonnage = b2cQuarter2Tonnage + 10;
                    b2cQuarter3Tonnage = b2cQuarter3Tonnage + 15;
                    b2cQuarter4Tonnage = b2cQuarter4Tonnage + 20;

                    //Adding B2C Data for all quarters
                    helper.CreateEeeOutputAmount(dataReturnVersion1, producer.RegisteredProducer, b2cObligationType, categoryId, b2cQuarter1Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion2, producer.RegisteredProducer, b2cObligationType, categoryId, b2cQuarter2Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion3, producer.RegisteredProducer, b2cObligationType, categoryId, b2cQuarter3Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion4, producer.RegisteredProducer, b2cObligationType, categoryId, b2cQuarter4Tonnage);

                    //Adding B2B Data for all quarters
                    helper.CreateEeeOutputAmount(dataReturnVersion1, producer.RegisteredProducer, b2bObligationType, categoryId, b2bQuarter1Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion2, producer.RegisteredProducer, b2bObligationType, categoryId, b2bQuarter2Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion3, producer.RegisteredProducer, b2bObligationType, categoryId, b2bQuarter3Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion4, producer.RegisteredProducer, b2bObligationType, categoryId, b2bQuarter4Tonnage);
                }

                db.Model.SaveChanges();
                var results = new List<ProducerEeeCsvData>();
                // Act
                results = await db.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, b2bObligationType, false, false);

                //Assert
                Assert.NotNull(results);

                ProducerEeeCsvData result = results[0];
                Assert.Equal(approvalNumber, result.ApprovalNumber);

                Assert.Equal(prn, result.PRN);
                Assert.Equal(1, result.Cat1Q1);
                Assert.Equal(2, result.Cat2Q1);
                Assert.Equal(3, result.Cat3Q1);
                Assert.Equal(4, result.Cat4Q1);
                Assert.Equal(5, result.Cat5Q1);
                Assert.Equal(6, result.Cat6Q1);
                Assert.Equal(7, result.Cat7Q1);
                Assert.Equal(8, result.Cat8Q1);
                Assert.Equal(9, result.Cat9Q1);
                Assert.Equal(10, result.Cat10Q1);
                Assert.Equal(11, result.Cat11Q1);
                Assert.Equal(12, result.Cat12Q1);
                Assert.Equal(13, result.Cat13Q1);
                Assert.Equal(14, result.Cat14Q1);
                Assert.Equal(15, result.Cat15Q1);

                Assert.Equal(2, result.Cat1Q2);
                Assert.Equal(4, result.Cat2Q2);
                Assert.Equal(6, result.Cat3Q2);
                Assert.Equal(8, result.Cat4Q2);
                Assert.Equal(10, result.Cat5Q2);
                Assert.Equal(12, result.Cat6Q2);
                Assert.Equal(14, result.Cat7Q2);
                Assert.Equal(16, result.Cat8Q2);
                Assert.Equal(18, result.Cat9Q2);
                Assert.Equal(20, result.Cat10Q2);
                Assert.Equal(22, result.Cat11Q2);
                Assert.Equal(24, result.Cat12Q2);
                Assert.Equal(26, result.Cat13Q2);
                Assert.Equal(28, result.Cat14Q2);
                Assert.Equal(30, result.Cat15Q2);

                Assert.Equal(3, result.Cat1Q3);
                Assert.Equal(6, result.Cat2Q3);
                Assert.Equal(9, result.Cat3Q3);
                Assert.Equal(12, result.Cat4Q3);
                Assert.Equal(15, result.Cat5Q3);
                Assert.Equal(18, result.Cat6Q3);
                Assert.Equal(21, result.Cat7Q3);
                Assert.Equal(24, result.Cat8Q3);
                Assert.Equal(27, result.Cat9Q3);
                Assert.Equal(30, result.Cat10Q3);
                Assert.Equal(33, result.Cat11Q3);
                Assert.Equal(36, result.Cat12Q3);
                Assert.Equal(39, result.Cat13Q3);
                Assert.Equal(42, result.Cat14Q3);
                Assert.Equal(45, result.Cat15Q3);

                Assert.Equal(4, result.Cat1Q4);
                Assert.Equal(8, result.Cat2Q4);
                Assert.Equal(12, result.Cat3Q4);
                Assert.Equal(16, result.Cat4Q4);
                Assert.Equal(20, result.Cat5Q4);
                Assert.Equal(24, result.Cat6Q4);
                Assert.Equal(28, result.Cat7Q4);
                Assert.Equal(32, result.Cat8Q4);
                Assert.Equal(36, result.Cat9Q4);
                Assert.Equal(40, result.Cat10Q4);
                Assert.Equal(44, result.Cat11Q4);
                Assert.Equal(48, result.Cat12Q4);
                Assert.Equal(52, result.Cat13Q4);
                Assert.Equal(56, result.Cat14Q4);
                Assert.Equal(60, result.Cat15Q4);

                Assert.Equal(1200, result.TotalTonnage);
            }
        }

        [Theory]
        [InlineData(ObligationType.B2B, ObligationType.B2C)]
        [InlineData(ObligationType.B2C, ObligationType.B2B)]
        public async Task Execute_HappyPath_ReturnsOnlyProducerEeeWithSelectedYearAndObligationType_As_WithAllCategories(ObligationType obligationTypeProducer1, ObligationType obligationTypeProducer2)
        {
            // this test relies on the obligation types being different, and not set to Both
            // the idea is we set producer 1 up with only the type we ask for, and producer 2 with the other type
            // and then ask for producer 1's obligation type, and ensure that's all we get

            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme = helper.CreateScheme();
                string approvalNumber = "WEE/TE0000ST/SCH";
                scheme.ApprovalNumber = approvalNumber;
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                int complianceYear = DateTime.Now.Year;
                memberUpload.ComplianceYear = complianceYear;
                string prn1 = "PRN123";
                string prn2 = "PRN456";
                string producer1ObligationType = EnumHelper.GetDisplayName(obligationTypeProducer1);
                string producer2ObligationType = EnumHelper.GetDisplayName(obligationTypeProducer2);

                var producer1 = helper.CreateProducerAsCompany(memberUpload, prn1);
                var producer2 = helper.CreateProducerAsCompany(memberUpload, prn2);

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, complianceYear, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, complianceYear, 2);
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme, complianceYear, 3);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme, complianceYear, 4);

                var categories = EnumHelper.GetValues(typeof(WeeeCategory));
                var maxCategoryId = categories.Max(x => x.Key);
                int quarter1Tonnage = 0;
                int quarter2Tonnage = 0;
                int quarter3Tonnage = 0;
                int quarter4Tonnage = 0;

                for (int categoryId = 1; categoryId <= maxCategoryId; categoryId++)
                {
                    quarter1Tonnage = quarter1Tonnage + 1;
                    quarter2Tonnage = quarter2Tonnage + 2;
                    quarter3Tonnage = quarter3Tonnage + 3;
                    quarter4Tonnage = quarter4Tonnage + 4;

                    // Add Data for all quarters to producer 1
                    helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, producer1ObligationType, categoryId, quarter1Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, producer1ObligationType, categoryId, quarter2Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion3, producer1.RegisteredProducer, producer1ObligationType, categoryId, quarter3Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion4, producer1.RegisteredProducer, producer1ObligationType, categoryId, quarter4Tonnage);

                    // Add Data for all quarters to producer 2
                    helper.CreateEeeOutputAmount(dataReturnVersion1, producer2.RegisteredProducer, producer2ObligationType, categoryId, quarter1Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion2, producer2.RegisteredProducer, producer2ObligationType, categoryId, quarter2Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion3, producer2.RegisteredProducer, producer2ObligationType, categoryId, quarter3Tonnage);
                    helper.CreateEeeOutputAmount(dataReturnVersion4, producer2.RegisteredProducer, producer2ObligationType, categoryId, quarter4Tonnage);
                }

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, producer1ObligationType, false, false);

                // Assert
                Assert.NotNull(results);

                // ensure there are no results for producer 2
                Assert.Null(results.Find(x => (x.PRN == prn2)));

                ProducerEeeCsvData result = results[0];
                Assert.Equal(approvalNumber, result.ApprovalNumber);

                Assert.Equal(prn1, result.PRN);

                Assert.Equal(1, result.Cat1Q1);
                Assert.Equal(2, result.Cat2Q1);
                Assert.Equal(3, result.Cat3Q1);
                Assert.Equal(4, result.Cat4Q1);
                Assert.Equal(5, result.Cat5Q1);
                Assert.Equal(6, result.Cat6Q1);
                Assert.Equal(7, result.Cat7Q1);
                Assert.Equal(8, result.Cat8Q1);
                Assert.Equal(9, result.Cat9Q1);
                Assert.Equal(10, result.Cat10Q1);
                Assert.Equal(11, result.Cat11Q1);
                Assert.Equal(12, result.Cat12Q1);
                Assert.Equal(13, result.Cat13Q1);
                Assert.Equal(14, result.Cat14Q1);
                Assert.Equal(15, result.Cat15Q1);

                Assert.Equal(2, result.Cat1Q2);
                Assert.Equal(4, result.Cat2Q2);
                Assert.Equal(6, result.Cat3Q2);
                Assert.Equal(8, result.Cat4Q2);
                Assert.Equal(10, result.Cat5Q2);
                Assert.Equal(12, result.Cat6Q2);
                Assert.Equal(14, result.Cat7Q2);
                Assert.Equal(16, result.Cat8Q2);
                Assert.Equal(18, result.Cat9Q2);
                Assert.Equal(20, result.Cat10Q2);
                Assert.Equal(22, result.Cat11Q2);
                Assert.Equal(24, result.Cat12Q2);
                Assert.Equal(26, result.Cat13Q2);
                Assert.Equal(28, result.Cat14Q2);
                Assert.Equal(30, result.Cat15Q2);

                Assert.Equal(3, result.Cat1Q3);
                Assert.Equal(6, result.Cat2Q3);
                Assert.Equal(9, result.Cat3Q3);
                Assert.Equal(12, result.Cat4Q3);
                Assert.Equal(15, result.Cat5Q3);
                Assert.Equal(18, result.Cat6Q3);
                Assert.Equal(21, result.Cat7Q3);
                Assert.Equal(24, result.Cat8Q3);
                Assert.Equal(27, result.Cat9Q3);
                Assert.Equal(30, result.Cat10Q3);
                Assert.Equal(33, result.Cat11Q3);
                Assert.Equal(36, result.Cat12Q3);
                Assert.Equal(39, result.Cat13Q3);
                Assert.Equal(42, result.Cat14Q3);
                Assert.Equal(45, result.Cat15Q3);

                Assert.Equal(4, result.Cat1Q4);
                Assert.Equal(8, result.Cat2Q4);
                Assert.Equal(12, result.Cat3Q4);
                Assert.Equal(16, result.Cat4Q4);
                Assert.Equal(20, result.Cat5Q4);
                Assert.Equal(24, result.Cat6Q4);
                Assert.Equal(28, result.Cat7Q4);
                Assert.Equal(32, result.Cat8Q4);
                Assert.Equal(36, result.Cat9Q4);
                Assert.Equal(40, result.Cat10Q4);
                Assert.Equal(44, result.Cat11Q4);
                Assert.Equal(48, result.Cat12Q4);
                Assert.Equal(52, result.Cat13Q4);
                Assert.Equal(56, result.Cat14Q4);
                Assert.Equal(60, result.Cat15Q4);

                Assert.Equal(1200, result.TotalTonnage);
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
                    await db.StoredProcedures.SpgProducerEeeCsvData(2000, null, "B2C", false, false);

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
                    "B2C",
                    false,
                    false);

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
                    "B2C",
                    false,
                    false);

                // Assert
                Assert.NotNull(results);

                Assert.Single(results);

                Assert.NotNull(results[0]);
                Assert.Equal("Test Scheme 1", results[0].SchemeName);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Execute_WithDirectRegistrantSubmissions_ReturnsResults(bool directRegistrantFilter)
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2060;
                // Direct registrant data is for the previous year
                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2B }, // Should be excluded as B2B
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, amounts1, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission);

                var (organisation2, directRegistrant2, registeredProducer2) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 2", "WEE/AG48365JX", complianceYear);

                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 4.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission2 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant2, registeredProducer2, complianceYear + 1, amounts2, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission2);

                // should not affect the sums as producer submission has been removed
                var (_, directRegistrant3, registeredProducer3) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 3", "WEE/AG28165JX", complianceYear);

                var amounts3 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.MedicalDevices, Amount = 4.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission3 = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant3, registeredProducer3, complianceYear + 1, amounts3, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission3);

                registeredProducer3.Remove();

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

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C", directRegistrantFilter, false);

                results.Should().NotBeNull();

                if (directRegistrantFilter)
                {
                    results.Count.Should().Be(2);

                    var expectedAmounts1 = new Dictionary<string, decimal> { { "Cat1Q4", 123.456m }, { "Cat4Q4", 2m } };
                    AssertEeeElementData(results.ElementAt(0), organisation1, registeredProducer1, country, expectedAmounts1, 125.456m);

                    var expectedAmounts2 = new Dictionary<string, decimal> { { "Cat8Q4", 4.456m } };
                    AssertEeeElementData(results.ElementAt(1), organisation2, registeredProducer2, country, expectedAmounts2, 4.456m);
                }
                else
                {
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
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantAndSchemeSubmissions_GivenOnlySchemeFilter_ShouldOnlyReturnSchemeResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                const int complianceYear = 2032;
                // Direct registrant data is for the previous years should be returned in the results
                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG44365JN", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, amounts1, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission);

                // Create a scheme that should only be returned in the results
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
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH1453NF/SCH", "WEE9462846",
                    Domain.Obligation.ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                var schemeRegisteredProducer1 =
                    new Domain.Producer.RegisteredProducer("WEE/AG18365JE", complianceYear, scheme1);

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

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C", false, true);

                results.Should().NotBeNull();
                results.Count.Should().Be(1);

                var schemeElement = results.ElementAt(0);
                schemeElement.SchemeName.Should().Be(scheme1.SchemeName);
                schemeElement.ApprovalNumber.Should().Be(scheme1.ApprovalNumber);
                schemeElement.Cat1Q1.Should().Be(123.457m);
                schemeElement.TotalTonnage.Should().Be(123.457m);
                schemeElement.PRN.Should().Be(schemeRegisteredProducer1.ProducerRegistrationNumber);
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantAndSchemeSubmissions_GivenOnlySchemeId_ShouldOnlyReturnSchemeResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                const int complianceYear = 2032;
                // Direct registrant data is for the previous years should be returned in the results
                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG44365JN", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, amounts1, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission);

                // Create a scheme that should only be returned in the results
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
                scheme1.UpdateScheme("Test Scheme 1", "WEE/AH3453NF/SCH", "WEE9462846",
                    Domain.Obligation.ObligationType.B2C, authority);
                scheme1.SetStatus(Domain.Scheme.SchemeStatus.Approved);

                var schemeRegisteredProducer1 =
                    new Domain.Producer.RegisteredProducer("WEE/AG17365JE", complianceYear, scheme1);

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

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, scheme1.Id, "B2C", false, false);

                results.Should().NotBeNull();
                results.Count.Should().Be(1);

                var schemeElement = results.ElementAt(0);
                schemeElement.SchemeName.Should().Be(scheme1.SchemeName);
                schemeElement.ApprovalNumber.Should().Be(scheme1.ApprovalNumber);
                schemeElement.Cat1Q1.Should().Be(123.457m);
                schemeElement.TotalTonnage.Should().Be(123.457m);
                schemeElement.PRN.Should().Be(schemeRegisteredProducer1.ProducerRegistrationNumber);
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsThatHaveBeenReturnedAndReSubmittedAndPaid_ShouldUseLatestSubmittedEEE()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2059;
                // Direct registrant data is for the previous year
                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 1m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, amounts1, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission);

                DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 2", "WEE/AG38365JX", complianceYear);

                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission);

                //update the amounts
                var amounts2 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 3m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                await DirectRegistrantHelper.SubmitSubmission(wrapper, submission, amounts2);

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C", false, false);

                results.Should().NotBeNull();

                results.Count.Should().Be(1);

                var expectedAmounts1 = new Dictionary<string, decimal> { { "Cat1Q4", 2m }, { "Cat4Q4", 3m } };
                AssertEeeElementData(results.ElementAt(0), organisation1, registeredProducer1, country, expectedAmounts1, 5m);
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsThatHaveBeenReturnedAndPaid_ShouldUseSubmittedEEE()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                var (_, country) = DirectRegistrantHelper.SetupCommonTestData(wrapper);

                const int complianceYear = 2053;

                var (organisation1, directRegistrant1, registeredProducer1) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts1 = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 1m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant1, registeredProducer1, complianceYear + 1, amounts1, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission);

                DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company 2", "WEE/AG37365JX", complianceYear);

                // should use the original amounts in the report as these amounts are not submitted
                var updatedAmounts = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 2m, ObligationType = Domain.Obligation.ObligationType.B2C },
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.ConsumerEquipment, Amount = 3m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                await DirectRegistrantHelper.ReturnSubmission(wrapper, submission);

                await DirectRegistrantHelper.UpdateEeeeAmounts(wrapper, submission, updatedAmounts);

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C", false, false);

                results.Should().NotBeNull();

                results.Count.Should().Be(1);

                var expectedAmounts1 = new Dictionary<string, decimal> { { "Cat1Q4", 1m }, { "Cat4Q4", 2m } };
                AssertEeeElementData(results.ElementAt(0), organisation1, registeredProducer1, country, expectedAmounts1, 3m);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Execute_WithDirectRegistrantSubmissionsWithIncorrectStatus_ReturnsEmptyResults(bool directRegistrantFilter)
        {
            using (var wrapper = new DatabaseWrapper())
            {
                DirectRegistrantHelper.SetupCommonTestData(wrapper);
                const int complianceYear = 2070;

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };
                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant, registeredProducer, complianceYear + 1, amounts, DirectProducerSubmissionStatus.Incomplete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission);

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C", directRegistrantFilter, false);

                results.Should().NotBeNull();
                results.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsWithNoComplianceYearMatch_ReturnsEmptyResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                const int complianceYear = 1010;

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", 1000);

                var amounts = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };
                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant, registeredProducer, 1000, amounts, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission);

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C", false, false);

                results.Should().NotBeNull();
                results.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsWithRemovedProducer_ReturnsEmptyResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                const int complianceYear = 1978;

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                var submission = await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant, registeredProducer, complianceYear + 1, amounts, DirectProducerSubmissionStatus.Complete);
                await DirectRegistrantHelper.SetSubmissionAsPaid(wrapper, submission);

                registeredProducer.Remove();

                await wrapper.WeeeContext.SaveChangesAsync();

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C", false, false);

                results.Should().NotBeNull();
                results.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Execute_WithDirectRegistrantSubmissionsThatHaveNotBeenPaid_ReturnsEmptyResults()
        {
            using (var wrapper = new DatabaseWrapper())
            {
                const int complianceYear = 1978;

                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(wrapper, "My company", "WEE/AG48365JN", complianceYear);

                var amounts = new List<DirectRegistrantHelper.EeeOutputAmountData>
                {
                    new DirectRegistrantHelper.EeeOutputAmountData { Category = WeeeCategory.LargeHouseholdAppliances, Amount = 123.456m, ObligationType = Domain.Obligation.ObligationType.B2C }
                };

                await DirectRegistrantHelper.CreateSubmission(wrapper, directRegistrant, registeredProducer, complianceYear + 1, amounts, DirectProducerSubmissionStatus.Complete, null, false);

                registeredProducer.Remove();

                await wrapper.WeeeContext.SaveChangesAsync();

                var results = await wrapper.WeeeContext.StoredProcedures.SpgProducerEeeCsvData(complianceYear, null, "B2C", false, false);

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
