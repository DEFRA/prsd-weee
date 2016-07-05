namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgProducerEeeDataHistoryCsvTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsProducerEeeDataHistory()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE0000ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer1.ObligationType = "B2B";
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 6);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 8);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, "B2B", 2, 200);

                var scheme2 = helper.CreateScheme();
                scheme2.ApprovalNumber = "WEE/TE0000S1/SCH";
                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2000;

                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "PRN123");
                producer2.ObligationType = "B2C";
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 9);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme2, 2000, 2);
                dataReturnVersion4.SubmittedDate = new DateTime(2015, 1, 10);

                helper.CreateEeeOutputAmount(dataReturnVersion3, producer2.RegisteredProducer, "B2C", 1, 40);
                helper.CreateEeeOutputAmount(dataReturnVersion4, producer2.RegisteredProducer, "B2C", 2, 1000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN123");

                //Assert
                Assert.NotNull(results);

                ProducerEeeHistoryCsvData.ProducerInReturnsResult b2cProducer = results.ProducerReturnsHistoryData.Find(x => (x.ApprovalNumber == "WEE/TE0000S1/SCH"));
                Assert.NotNull(b2cProducer);
                Assert.Equal(2000, b2cProducer.ComplianceYear);
                Assert.Equal(1000, b2cProducer.Cat2B2C);
                Assert.Equal("Yes", b2cProducer.LatestData);

                ProducerEeeHistoryCsvData.ProducerInReturnsResult b2bProducer = results.ProducerReturnsHistoryData.Find(x => (x.ApprovalNumber == "WEE/TE0000ST/SCH"));
                Assert.NotNull(b2bProducer);
                Assert.Equal(2000, b2bProducer.ComplianceYear);
                Assert.Equal(200, b2bProducer.Cat2B2B);
                Assert.Null(b2bProducer.Cat2B2C);
                Assert.Equal("Yes", b2bProducer.LatestData);

                Assert.Equal(4, results.ProducerReturnsHistoryData.Count);
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_ForEachQuarter_ReturnsProducerEeeDataHistoryLatestDataSetToYes()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE1111ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN345");
                producer1.ObligationType = "B2B";
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 4);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 8);

                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 6);

                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2000, 3);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion4.SubmittedDate = new DateTime(2015, 1, 1);
                //Latest for quarter 4
                var dataReturnVersion5 = helper.CreateDataReturnVersion(scheme1, 2000, 4);
                dataReturnVersion5.SubmittedDate = new DateTime(2015, 1, 10);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 10, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, "B2B", 11, 200);
                helper.CreateEeeOutputAmount(dataReturnVersion3, producer1.RegisteredProducer, "B2B", 12, 300);
                helper.CreateEeeOutputAmount(dataReturnVersion4, producer1.RegisteredProducer, "B2B", 13, 500);
                helper.CreateEeeOutputAmount(dataReturnVersion5, producer1.RegisteredProducer, "B2B", 13, 600);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN345");

                //Assert
                Assert.NotNull(results);
                Assert.Equal("Yes", results.ProducerReturnsHistoryData[4].LatestData);
                Assert.Equal(600, results.ProducerReturnsHistoryData[4].Cat13B2B);

                Assert.Equal("No", results.ProducerReturnsHistoryData[3].LatestData);
                Assert.Equal(100, results.ProducerReturnsHistoryData[3].Cat10B2B);

                Assert.Equal("Yes", results.ProducerReturnsHistoryData[2].LatestData);
                Assert.Equal(200, results.ProducerReturnsHistoryData[2].Cat11B2B);

                Assert.Equal("Yes", results.ProducerReturnsHistoryData[1].LatestData);
                Assert.Equal(300, results.ProducerReturnsHistoryData[1].Cat12B2B);

                Assert.Equal("Yes", results.ProducerReturnsHistoryData[0].LatestData);
                Assert.Equal(500, results.ProducerReturnsHistoryData[0].Cat13B2B);

                Assert.Equal(5, results.ProducerReturnsHistoryData.Count);
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_ForProducerRegisteredWith2DifferentSchemesAndDifferentObligationType_ReturnsProducerEeeDataHistoryLatestDataSetToYes()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE1234ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN567");
                producer1.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);
                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);

                var scheme2 = helper.CreateScheme();
                scheme2.ApprovalNumber = "WEE/TE2345ST/SCH";

                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2000;

                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "PRN567");
                producer2.ObligationType = "B2C";

                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer2.RegisteredProducer, "B2C", 1, 200);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN567");

                // Assert
                Assert.NotNull(results);
                Assert.Equal(2, results.ProducerReturnsHistoryData.Count);
                Assert.Equal("WEE/TE2345ST/SCH", results.ProducerReturnsHistoryData[1].ApprovalNumber);
                Assert.Equal("Yes", results.ProducerReturnsHistoryData[1].LatestData);
                Assert.Equal("WEE/TE1234ST/SCH", results.ProducerReturnsHistoryData[0].ApprovalNumber);
                Assert.Equal("Yes", results.ProducerReturnsHistoryData[0].LatestData);
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_ReturnsProducerEeeDataHistoryForChangedTonnageValues()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                // Create two submissions with unchanged EEE output amounts
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);

                EeeOutputAmount eeeOutputAmount1 =
                    helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2B", 1, 100);
                EeeOutputReturnVersion eeeOutputReturnVersion1 = 
                    helper.CreateEeeOutputReturnVersion();
                helper.AddEeeOutputAmount(eeeOutputReturnVersion1, eeeOutputAmount1);

                dataReturnVersion1.EeeOutputReturnVersion = eeeOutputReturnVersion1;
                dataReturnVersion2.EeeOutputReturnVersion = eeeOutputReturnVersion1;

                // Create a third submission with changed tonnage for the EEE output amount
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);

                EeeOutputAmount eeeOutputAmount2 =
                    helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2B", 1, 200);
                EeeOutputReturnVersion eeeOutputReturnVersion2 =
                    helper.CreateEeeOutputReturnVersion();
                helper.AddEeeOutputAmount(eeeOutputReturnVersion2, eeeOutputAmount2);

                dataReturnVersion3.EeeOutputReturnVersion = eeeOutputReturnVersion2;

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                // Assert
                Assert.NotNull(results);
                // Only shows entries for tonnage value changes and ignores the ones with no change.
                Assert.Equal(2, results.ProducerReturnsHistoryData.Count);

                Assert.Collection(results.ProducerReturnsHistoryData,
                   (r1) => Assert.Equal(new DateTime(2015, 1, 1), r1.SubmittedDate),
                   (r2) => Assert.Equal(new DateTime(2015, 1, 3), r2.SubmittedDate));

                Assert.Collection(results.ProducerReturnsHistoryData,
                  (r1) => Assert.Equal("No", r1.LatestData),
                  (r2) => Assert.Equal("Yes", r2.LatestData));
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_ReturnsProducerEeeDataHistoryOrderByCYAscending()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE2222ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN111");
                producer1.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2001, 1);
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 1999, 1);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme1, 2005, 1);
                var dataReturnVersion5 = helper.CreateDataReturnVersion(scheme1, 2004, 1);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, "B2B", 1, 200);
                helper.CreateEeeOutputAmount(dataReturnVersion3, producer1.RegisteredProducer, "B2B", 1, 300);
                helper.CreateEeeOutputAmount(dataReturnVersion4, producer1.RegisteredProducer, "B2B", 4, 400);
                helper.CreateEeeOutputAmount(dataReturnVersion5, producer1.RegisteredProducer, "B2B", 4, 500);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN111");

                //Assert
                Assert.NotNull(results);
                Assert.Equal(5, results.ProducerReturnsHistoryData.Count);

                Assert.Collection(results.ProducerReturnsHistoryData,
                    (r1) => Assert.Equal(1999, r1.ComplianceYear),
                    (r2) => Assert.Equal(2000, r2.ComplianceYear),
                    (r3) => Assert.Equal(2001, r3.ComplianceYear),
                    (r4) => Assert.Equal(2004, r4.ComplianceYear),
                    (r5) => Assert.Equal(2005, r5.ComplianceYear));
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_OmitsDataReturnsNotChanged()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                // Create a submission
                EeeOutputAmount eeeOutputAmount1 =
                    helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2B", 1, 100);
                EeeOutputReturnVersion eeeOutputReturnVersion1 = helper.CreateEeeOutputReturnVersion();
                helper.AddEeeOutputAmount(eeeOutputReturnVersion1, eeeOutputAmount1);

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                dataReturnVersion1.EeeOutputReturnVersion = eeeOutputReturnVersion1;

                // Create two additional submissions, both having the same data
                // but are different from the first
                EeeOutputAmount eeeOutputAmount2 =
                    helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2B", 1, 200);
                EeeOutputReturnVersion eeeOutputReturnVersion2 = helper.CreateEeeOutputReturnVersion();
                helper.AddEeeOutputAmount(eeeOutputReturnVersion2, eeeOutputAmount2);

                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);

                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);

                dataReturnVersion2.EeeOutputReturnVersion = eeeOutputReturnVersion2;
                dataReturnVersion3.EeeOutputReturnVersion = eeeOutputReturnVersion2;

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                //Assert
                Assert.NotNull(results);
                //Only shows entries for tonnage value changes and ignores the ones with no change.
                Assert.Equal(2, results.ProducerReturnsHistoryData.Count);

                Assert.Collection(results.ProducerReturnsHistoryData,
                   (r1) => Assert.Equal(new DateTime(2015, 1, 1), r1.SubmittedDate),
                   (r2) => Assert.Equal(new DateTime(2015, 1, 2), r2.SubmittedDate));

                Assert.Collection(results.ProducerReturnsHistoryData,
                  (r1) => Assert.Equal("No", r1.LatestData),
                  (r2) => Assert.Equal("Yes", r2.LatestData));
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_JustOneProducerUploadForTheScheme()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion.SubmittedDate = new DateTime(2015, 1, 1);
                dataReturnVersion.DataReturn.Quarter = 4;

                helper.CreateEeeOutputAmount(dataReturnVersion, producer1.RegisteredProducer, "B2B", 1, 100);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                //Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.ProducerReturnsHistoryData.Count);
                Assert.Equal("PRN897", results.ProducerReturnsHistoryData[0].PRN);
                Assert.Equal("WEE/TE3334ST/SCH", results.ProducerReturnsHistoryData[0].ApprovalNumber);
                Assert.Equal("Yes", results.ProducerReturnsHistoryData[0].LatestData);
                Assert.Equal(4, results.ProducerReturnsHistoryData[0].Quarter);
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_SchemeWith2ProducersRemovesProducer1_ReturnsLatestEntryWithNULLTonnageValues()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer2.ObligationType = "B2B";

                // Create a data return version with submission for 2 producers
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion1, producer2.RegisteredProducer, "B2B", 1, 200);

                // Second data return version with producer 1 removed and only producer 2 data
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);

                helper.CreateEeeOutputAmount(dataReturnVersion2, producer2.RegisteredProducer, "B2B", 1, 300);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                // Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.ProducerRemovedFromReturnsData.Count);

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                   (r1) => Assert.Equal(new DateTime(2015, 1, 2), r1.SubmittedDate));

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                  (r1) => Assert.Equal("WEE/TE3334ST/SCH", r1.ApprovalNumber));

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                   (r1) => Assert.Equal(1, r1.Quarter));
            }
        }

        [Fact]
        public async void SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_SubmissionWithNoEeeOutputAmounts_ReturnsPreviousProducerDataAsRemoved()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "PRN898");
                producer2.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion1, producer2.RegisteredProducer, "B2B", 2, 100);

                // Second upload with no EEE output amounts
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);
                dataReturnVersion2.EeeOutputReturnVersionId = null;
                dataReturnVersion2.EeeOutputReturnVersion = null;

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                // Assert
                Assert.NotNull(results);

                Assert.Equal(1, results.ProducerRemovedFromReturnsData.Count);

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                   (r1) => Assert.Equal(new DateTime(2015, 1, 2), r1.SubmittedDate));

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                  (r1) => Assert.Equal("WEE/TE3334ST/SCH", r1.ApprovalNumber));

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                   (r1) => Assert.Equal(1, r1.Quarter));
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_SchemeRemovesProducerAfterFirstUploadAndAddLaterAgainInThirdUpload_Returns3RowEvenIfDataIsSame()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer2.ObligationType = "B2B";

                EeeOutputAmount eeeOutputAmount1 =
                    helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2B", 1, 100);

                EeeOutputAmount eeeOutputAmount2 =
                    helper.CreateEeeOutputAmount(producer2.RegisteredProducer, "B2B", 1, 200);

                // Create a data return version with submission for 2 producers
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                helper.AddEeeOutputAmount(dataReturnVersion1, eeeOutputAmount1);
                helper.AddEeeOutputAmount(dataReturnVersion1, eeeOutputAmount2);

                // Second data return version with producer 1 removed 
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);

                helper.AddEeeOutputAmount(dataReturnVersion2, eeeOutputAmount2);

                // Third data return version with producer 1 added back again
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);

                helper.CreateEeeOutputAmount(dataReturnVersion3, producer1.RegisteredProducer, "B2B", 1, 100);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                //Assert
                Assert.NotNull(results);

                Assert.Equal(2, results.ProducerReturnsHistoryData.Count);

                Assert.Collection(results.ProducerReturnsHistoryData,
                   (r1) => Assert.Equal(new DateTime(2015, 1, 1), r1.SubmittedDate),
                   (r2) => Assert.Equal(new DateTime(2015, 1, 3), r2.SubmittedDate));

                Assert.Equal(1, results.ProducerRemovedFromReturnsData.Count);

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                   (r1) => Assert.Equal(new DateTime(2015, 1, 2), r1.SubmittedDate));

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                  (r1) => Assert.Equal("WEE/TE3334ST/SCH", r1.ApprovalNumber));

                Assert.Collection(results.ProducerRemovedFromReturnsData,
                   (r1) => Assert.Equal(1, r1.Quarter));
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_DataReturnNotSubmitted_RetunsNoRowsForProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = null;
                dataReturnVersion1.DataReturn.Quarter = 4;

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                //Assert
                Assert.Equal(0, results.ProducerReturnsHistoryData.Count);
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_ReturnsOneRowForChangedDataEvenIfOneCategoryChanged()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                EeeOutputAmount eeeOutputAmount = helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2B", 2, 200);

                helper.AddEeeOutputAmount(dataReturnVersion1, eeeOutputAmount);
                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);

                // Second upload with only category 1 changed, category 2 remains unchanged
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);

                helper.AddEeeOutputAmount(dataReturnVersion2, eeeOutputAmount);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, "B2B", 1, 300);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                // Assert
                Assert.NotNull(results);
                // Only shows entries for tonnage value changes and ignores the ones with no change.
                Assert.Equal(2, results.ProducerReturnsHistoryData.Count);

                Assert.Collection(results.ProducerReturnsHistoryData,
                   (r1) => Assert.Equal(new DateTime(2015, 1, 1), r1.SubmittedDate),
                   (r2) => Assert.Equal(new DateTime(2015, 1, 2), r2.SubmittedDate));

                Assert.Collection(results.ProducerReturnsHistoryData,
                  (r1) => Assert.Equal("No", r1.LatestData),
                  (r2) => Assert.Equal("Yes", r2.LatestData));

                Assert.Collection(results.ProducerReturnsHistoryData,
                  (r1) => Assert.Equal(100, r1.Cat1B2B),
                  (r2) => Assert.Equal(300, r2.Cat1B2B));

                Assert.Collection(results.ProducerReturnsHistoryData,
                  (r1) => Assert.Equal(200, r1.Cat2B2B),
                  (r2) => Assert.Equal(200, r2.Cat2B2B));
            }
        }

        [Fact]
        public async void SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_ProducerHasExistingEeeAndThenOneCategoryRemoved_ReturnsRowsWithAndWithoutRemovedCategory()
        {
            var complianceYear = 2995;
            var producerRegistrationNumber = "WEE/AW0101AW";

            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = complianceYear;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, producerRegistrationNumber);

                var eeeOutputAmount1 = helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2C", 1, 101);
                var eeeOutputAmount2 = helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2C", 2, 102);
                var eeeOutputAmount3 = helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2B", 3, 203);

                // Create first upload with two categories
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, complianceYear, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                helper.AddEeeOutputAmount(dataReturnVersion1, eeeOutputAmount1);
                helper.AddEeeOutputAmount(dataReturnVersion1, eeeOutputAmount2);
                helper.AddEeeOutputAmount(dataReturnVersion1, eeeOutputAmount3);

                // Create second upload with one of the original categories removed
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, complianceYear, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 2, 1);

                helper.AddEeeOutputAmount(dataReturnVersion2, eeeOutputAmount1);
                helper.AddEeeOutputAmount(dataReturnVersion2, eeeOutputAmount3);

                database.Model.SaveChanges();

                // Act
                var results = await database.StoredProcedures.SpgProducerEeeHistoryCsvData(producerRegistrationNumber);

                // Assert
                Assert.Equal(2, results.ProducerReturnsHistoryData.Count);

                // Check first row
                var result1 = results.ProducerReturnsHistoryData.First();

                Assert.Equal(producerRegistrationNumber, result1.PRN);
                Assert.Equal(1, result1.Quarter);
                Assert.Equal(complianceYear, result1.ComplianceYear);

                Assert.Equal(101, result1.Cat1B2C);
                Assert.Equal(102, result1.Cat2B2C);

                // Check second row
                var result2 = results.ProducerReturnsHistoryData.Last();

                Assert.Equal(producerRegistrationNumber, result2.PRN);
                Assert.Equal(1, result2.Quarter);
                Assert.Equal(complianceYear, result2.ComplianceYear);

                Assert.Equal(101, result2.Cat1B2C);
                Assert.Null(result2.Cat2B2C);
            }
        }

        [Fact]
        public async void SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_SecondSubmissionAffectsOtherProducerWhileCurrentProducerRemainsUnchanged_ReturnsOnlyOneRowForProducer()
        {
            var complianceYear = 2995;
            var producerRegistrationNumber1 = "WEE/AW0101AW";
            var producerRegistrationNumber2 = "WEE/AW0102AW";

            using (var database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = complianceYear;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, producerRegistrationNumber1);
                var producer2 = helper.CreateProducerAsCompany(memberUpload, producerRegistrationNumber2);

                var eeeOutputAmount1 = helper.CreateEeeOutputAmount(producer1.RegisteredProducer, "B2C", 1, 101);
                var eeeOutputAmount2 = helper.CreateEeeOutputAmount(producer2.RegisteredProducer, "B2C", 2, 102);
                var eeeOutputAmount3 = helper.CreateEeeOutputAmount(producer2.RegisteredProducer, "B2C", 2, 112);

                // Create first upload
                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, complianceYear, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                helper.AddEeeOutputAmount(dataReturnVersion1, eeeOutputAmount1);
                helper.AddEeeOutputAmount(dataReturnVersion1, eeeOutputAmount2);

                // Create second upload with tonnage value changing for producer 2 only
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, complianceYear, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 2, 1);

                helper.AddEeeOutputAmount(dataReturnVersion2, eeeOutputAmount1);
                helper.AddEeeOutputAmount(dataReturnVersion2, eeeOutputAmount3);

                database.Model.SaveChanges();

                // Act
                var results = await database.StoredProcedures.SpgProducerEeeHistoryCsvData(producerRegistrationNumber1);

                // Assert
                Assert.Equal(1, results.ProducerReturnsHistoryData.Count);

                // Check first row
                var result = results.ProducerReturnsHistoryData.First();

                Assert.Equal(producerRegistrationNumber1, result.PRN);
                Assert.Equal(1, result.Quarter);
                Assert.Equal(complianceYear, result.ComplianceYear);
                Assert.Equal(101, result.Cat1B2C);
                Assert.Equal("Yes", result.LatestData);
                Assert.Equal(new DateTime(2015, 1, 1), result.SubmittedDate);
            }
        }
    }
}