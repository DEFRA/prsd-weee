namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
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
                dataReturnVersion1.SubmittedDate = new System.DateTime(2015, 1, 6);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);
                dataReturnVersion2.SubmittedDate = new System.DateTime(2015, 1, 8);

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);
                helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, "B2B", 2, 200);

                var scheme2 = helper.CreateScheme();
                scheme2.ApprovalNumber = "WEE/TE0000S1/SCH";
                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2000;

                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "PRN123");
                producer2.ObligationType = "B2C";
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                dataReturnVersion1.SubmittedDate = new System.DateTime(2015, 1, 9);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme2, 2000, 2);
                dataReturnVersion4.SubmittedDate = new System.DateTime(2015, 1, 10);

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
                dataReturnVersion1.SubmittedDate = new System.DateTime(2015, 1, 8);

                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);
                dataReturnVersion2.SubmittedDate = new System.DateTime(2015, 1, 6);

                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2000, 3);
                dataReturnVersion3.SubmittedDate = new System.DateTime(2015, 1, 3);
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion4.SubmittedDate = new System.DateTime(2015, 1, 1);
                //Latest for quarter 4
                var dataReturnVersion5 = helper.CreateDataReturnVersion(scheme1, 2000, 4);
                dataReturnVersion5.SubmittedDate = new System.DateTime(2015, 1, 10);

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
        public async Task SpgProducerEeeDataHistoryCsvTests_ForProducerRegisteredWith2DifferentSchemeAnddifferentObligationType_ReturnsProducerEeeDataHistoryLatestDataSetToYes()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE1234ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN567");
                producer1.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                EeeOutputAmount eeeOutputAmount = new EeeOutputAmount();
                eeeOutputAmount.Id = Guid.NewGuid();
                eeeOutputAmount.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount.WeeeCategory = 1;
                eeeOutputAmount.Tonnage = 100;
                eeeOutputAmount.ObligationType = "B2B";

                EeeOutputReturnVersion version = new EeeOutputReturnVersion();
                version.Id = Guid.NewGuid();

                EeeOutputReturnVersionAmount versionAmount = new EeeOutputReturnVersionAmount();

                versionAmount.EeeOutputAmount = eeeOutputAmount;
                versionAmount.EeeOutputReturnVersion = version;

                version.EeeOutputReturnVersionAmounts.Add(versionAmount);
                dataReturnVersion1.EeeOutputReturnVersion = version;

                var scheme2 = helper.CreateScheme();
                scheme2.ApprovalNumber = "WEE/TE2345ST/SCH";
                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2000;
                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "PRN567");
                producer2.ObligationType = "B2C";

                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme2, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);
                EeeOutputAmount eeeOutputAmount1 = new EeeOutputAmount();
                eeeOutputAmount1.Id = Guid.NewGuid();
                eeeOutputAmount1.RegisteredProducer = producer2.RegisteredProducer;
                eeeOutputAmount1.WeeeCategory = 1;
                eeeOutputAmount1.Tonnage = 200;
                eeeOutputAmount1.ObligationType = "B2C";

                EeeOutputReturnVersion version1 = new EeeOutputReturnVersion();
                version1.Id = Guid.NewGuid();

                EeeOutputReturnVersionAmount versionAmount1 = new EeeOutputReturnVersionAmount();

                versionAmount1.EeeOutputAmount = eeeOutputAmount1;
                versionAmount1.EeeOutputReturnVersion = version1;
                version1.EeeOutputReturnVersionAmounts.Add(versionAmount1);
                dataReturnVersion2.EeeOutputReturnVersion = version1;
                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN567");

                //Assert
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
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);
                EeeOutputAmount eeeOutputAmount = new EeeOutputAmount();
                eeeOutputAmount.Id = Guid.NewGuid();
                eeeOutputAmount.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount.WeeeCategory = 1;
                eeeOutputAmount.Tonnage = 100;
                eeeOutputAmount.ObligationType = "B2B";

                EeeOutputReturnVersion version = new EeeOutputReturnVersion();
                version.Id = Guid.NewGuid();

                EeeOutputReturnVersionAmount versionAmount = new EeeOutputReturnVersionAmount();

                versionAmount.EeeOutputAmount = eeeOutputAmount;
                versionAmount.EeeOutputReturnVersion = version;

                version.EeeOutputReturnVersionAmounts.Add(versionAmount);
                EeeOutputAmount eeeOutputAmount1 = new EeeOutputAmount();
                eeeOutputAmount1.Id = Guid.NewGuid();
                eeeOutputAmount1.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount1.WeeeCategory = 1;
                eeeOutputAmount1.Tonnage = 200;
                eeeOutputAmount1.ObligationType = "B2B";
                EeeOutputReturnVersion version1 = new EeeOutputReturnVersion();
                version1.Id = Guid.NewGuid();
                EeeOutputReturnVersionAmount versionAmount1 = new EeeOutputReturnVersionAmount();
                versionAmount1.EeeOutputAmount = eeeOutputAmount1;
                versionAmount1.EeeOutputReturnVersion = version1;

                version1.EeeOutputReturnVersionAmounts.Add(versionAmount1);

                dataReturnVersion1.EeeOutputReturnVersion = version;
                dataReturnVersion2.EeeOutputReturnVersion = version;
                dataReturnVersion3.EeeOutputReturnVersion = version1;

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                //Assert
                Assert.NotNull(results);
                //Only shows entries for tonnage value changes and ignores the ones with no change.
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
                //Arrange
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

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);

                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);

                EeeOutputAmount eeeOutputAmount = new EeeOutputAmount();
                eeeOutputAmount.Id = Guid.NewGuid();
                eeeOutputAmount.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount.WeeeCategory = 1;
                eeeOutputAmount.Tonnage = 100;
                eeeOutputAmount.ObligationType = "B2B";

                EeeOutputReturnVersion version = new EeeOutputReturnVersion();
                version.Id = Guid.NewGuid();

                EeeOutputReturnVersionAmount versionAmount = new EeeOutputReturnVersionAmount();

                versionAmount.EeeOutputAmount = eeeOutputAmount;
                versionAmount.EeeOutputReturnVersion = version;

                version.EeeOutputReturnVersionAmounts.Add(versionAmount);

                EeeOutputAmount eeeOutputAmount1 = new EeeOutputAmount();
                eeeOutputAmount1.Id = Guid.NewGuid();
                eeeOutputAmount1.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount1.WeeeCategory = 1;
                eeeOutputAmount1.Tonnage = 200;
                eeeOutputAmount1.ObligationType = "B2B";

                EeeOutputReturnVersion version1 = new EeeOutputReturnVersion();
                version1.Id = Guid.NewGuid();
                EeeOutputReturnVersionAmount versionAmount1 = new EeeOutputReturnVersionAmount();
                versionAmount1.EeeOutputAmount = eeeOutputAmount1;
                versionAmount1.EeeOutputReturnVersion = version1;

                version1.EeeOutputReturnVersionAmounts.Add(versionAmount1);

                dataReturnVersion1.EeeOutputReturnVersion = version;
                dataReturnVersion2.EeeOutputReturnVersion = version1;
                dataReturnVersion3.EeeOutputReturnVersion = version1;

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

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);
                dataReturnVersion1.DataReturn.Quarter = 4;

                EeeOutputAmount eeeOutputAmount = new EeeOutputAmount();
                eeeOutputAmount.Id = Guid.NewGuid();
                eeeOutputAmount.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount.WeeeCategory = 1;
                eeeOutputAmount.Tonnage = 100;
                eeeOutputAmount.ObligationType = "B2B";

                EeeOutputReturnVersion version = new EeeOutputReturnVersion();
                version.Id = Guid.NewGuid();

                EeeOutputReturnVersionAmount versionAmount = new EeeOutputReturnVersionAmount();

                versionAmount.EeeOutputAmount = eeeOutputAmount;
                versionAmount.EeeOutputReturnVersion = version;

                version.EeeOutputReturnVersionAmounts.Add(versionAmount);

                dataReturnVersion1.EeeOutputReturnVersion = version;

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
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer2.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                EeeOutputAmount eeeOutputAmount = new EeeOutputAmount();
                eeeOutputAmount.Id = Guid.NewGuid();
                eeeOutputAmount.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount.WeeeCategory = 1;
                eeeOutputAmount.Tonnage = 100;
                eeeOutputAmount.ObligationType = "B2B";

                EeeOutputAmount eeeOutputAmount1 = new EeeOutputAmount();
                eeeOutputAmount1.Id = Guid.NewGuid();
                eeeOutputAmount1.RegisteredProducer = producer2.RegisteredProducer;
                eeeOutputAmount1.WeeeCategory = 1;
                eeeOutputAmount1.Tonnage = 200;
                eeeOutputAmount1.ObligationType = "B2B";

                EeeOutputReturnVersion version = new EeeOutputReturnVersion();
                version.Id = Guid.NewGuid();

                EeeOutputReturnVersionAmount versionAmount1 = new EeeOutputReturnVersionAmount();

                versionAmount1.EeeOutputAmount = eeeOutputAmount;
                versionAmount1.EeeOutputReturnVersion = version;

                version.EeeOutputReturnVersionAmounts.Add(versionAmount1);

                EeeOutputReturnVersionAmount versionAmount2 = new EeeOutputReturnVersionAmount();

                versionAmount2.EeeOutputAmount = eeeOutputAmount1;
                versionAmount2.EeeOutputReturnVersion = version;
                version.EeeOutputReturnVersionAmounts.Add(versionAmount2);

                dataReturnVersion1.EeeOutputReturnVersion = version;

                //Second data return version with producer 1 removed and only producer 2 data
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);

                EeeOutputAmount eeeOutputAmount3 = new EeeOutputAmount();
                eeeOutputAmount3.Id = Guid.NewGuid();
                eeeOutputAmount3.RegisteredProducer = producer2.RegisteredProducer;
                eeeOutputAmount3.WeeeCategory = 1;
                eeeOutputAmount3.Tonnage = 300;
                eeeOutputAmount3.ObligationType = "B2B";

                EeeOutputReturnVersion version1 = new EeeOutputReturnVersion();
                version1.Id = Guid.NewGuid();
                EeeOutputReturnVersionAmount versionAmount3 = new EeeOutputReturnVersionAmount();
                versionAmount3.EeeOutputAmount = eeeOutputAmount3;
                versionAmount3.EeeOutputReturnVersion = version1;

                version1.EeeOutputReturnVersionAmounts.Add(versionAmount3);
                dataReturnVersion2.EeeOutputReturnVersion = version1;

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                //Assert
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
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_SchemeRemovesProducerAfterFirstUploadandAddlateronInthirdUpload_Returns3RowEvenIfDataIsSame()
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

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "PRN123");
                producer2.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                EeeOutputAmount eeeOutputAmount = new EeeOutputAmount();
                eeeOutputAmount.Id = Guid.NewGuid();
                eeeOutputAmount.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount.WeeeCategory = 1;
                eeeOutputAmount.Tonnage = 100;
                eeeOutputAmount.ObligationType = "B2B";

                EeeOutputAmount eeeOutputAmount1 = new EeeOutputAmount();
                eeeOutputAmount1.Id = Guid.NewGuid();
                eeeOutputAmount1.RegisteredProducer = producer2.RegisteredProducer;
                eeeOutputAmount1.WeeeCategory = 1;
                eeeOutputAmount1.Tonnage = 200;
                eeeOutputAmount1.ObligationType = "B2B";

                EeeOutputReturnVersion version1 = new EeeOutputReturnVersion();
                version1.Id = Guid.NewGuid();

                EeeOutputReturnVersionAmount versionAmount1 = new EeeOutputReturnVersionAmount();

                versionAmount1.EeeOutputAmount = eeeOutputAmount;
                versionAmount1.EeeOutputReturnVersion = version1;

                version1.EeeOutputReturnVersionAmounts.Add(versionAmount1);
                dataReturnVersion1.EeeOutputReturnVersion = version1;

                //Second data return version with producer 1 removed 
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);
                
                EeeOutputReturnVersion version2 = new EeeOutputReturnVersion();
                version2.Id = Guid.NewGuid();
                EeeOutputReturnVersionAmount versionAmount2 = new EeeOutputReturnVersionAmount();
                versionAmount2.EeeOutputAmount = eeeOutputAmount1;
                versionAmount2.EeeOutputReturnVersion = version2;

                version2.EeeOutputReturnVersionAmounts.Add(versionAmount2);
                dataReturnVersion2.EeeOutputReturnVersion = version2;               
               
                //third data return version with producer 1 added back again
                var dataReturnVersion3 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion3.SubmittedDate = new DateTime(2015, 1, 3);

                EeeOutputAmount eeeOutputAmount2 = new EeeOutputAmount();
                eeeOutputAmount2.Id = Guid.NewGuid();
                eeeOutputAmount2.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount2.WeeeCategory = 1;
                eeeOutputAmount2.Tonnage = 100;
                eeeOutputAmount2.ObligationType = "B2B";

                EeeOutputReturnVersion version3 = new EeeOutputReturnVersion();
                version3.Id = Guid.NewGuid();
                EeeOutputReturnVersionAmount versionAmount3 = new EeeOutputReturnVersionAmount();
                versionAmount3.EeeOutputAmount = eeeOutputAmount2;
                versionAmount3.EeeOutputReturnVersion = version3;

                version3.EeeOutputReturnVersionAmounts.Add(versionAmount3);
                dataReturnVersion3.EeeOutputReturnVersion = version3;

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
                //Arrange
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

                EeeOutputAmount eeeOutputAmount = new EeeOutputAmount();
                eeeOutputAmount.Id = Guid.NewGuid();
                eeeOutputAmount.RegisteredProducer = producer1.RegisteredProducer;
                eeeOutputAmount.WeeeCategory = 1;
                eeeOutputAmount.Tonnage = 100;
                eeeOutputAmount.ObligationType = "B2B";

                EeeOutputReturnVersion version = new EeeOutputReturnVersion();
                version.Id = Guid.NewGuid();

                EeeOutputReturnVersionAmount versionAmount = new EeeOutputReturnVersionAmount();

                versionAmount.EeeOutputAmount = eeeOutputAmount;
                versionAmount.EeeOutputReturnVersion = version;

                version.EeeOutputReturnVersionAmounts.Add(versionAmount);

                dataReturnVersion1.EeeOutputReturnVersion = version;

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
                //Arrange
                ModelHelper helper = new ModelHelper(db.Model);
                var scheme1 = helper.CreateScheme();
                scheme1.ApprovalNumber = "WEE/TE3334ST/SCH";
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2000;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "PRN897");
                producer1.ObligationType = "B2B";

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion1.SubmittedDate = new DateTime(2015, 1, 1);

                EeeOutputAmount eeeOutputAmount = helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);
                EeeOutputAmount eeeOutputAmount1 = helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 2, 200);
                
                //Second upload with only category 1 changed, category 2 remains unchanges
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 1);
                dataReturnVersion2.SubmittedDate = new DateTime(2015, 1, 2);

                EeeOutputAmount eeeOutputAmount3 = helper.CreateEeeOutputAmount(dataReturnVersion2, producer1.RegisteredProducer, "B2B", 1, 300);

                dataReturnVersion2.EeeOutputReturnVersion.EeeOutputReturnVersionAmounts.Add(new EeeOutputReturnVersionAmount
                {
                    EeeOuputAmountId = eeeOutputAmount1.Id,
                    EeeOutputAmount = eeeOutputAmount1,
                    EeeOutputReturnVersionId = dataReturnVersion2.EeeOutputReturnVersion.Id,
                    EeeOutputReturnVersion = dataReturnVersion2.EeeOutputReturnVersion
                });
               
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

                Assert.Collection(results.ProducerReturnsHistoryData,
                  (r1) => Assert.Equal(100, r1.Cat1B2B),
                  (r2) => Assert.Equal(300, r2.Cat1B2B));

                Assert.Collection(results.ProducerReturnsHistoryData,
                  (r1) => Assert.Equal(200, r1.Cat2B2B),
                  (r2) => Assert.Equal(200, r2.Cat2B2B));
            }
        }
    }
}