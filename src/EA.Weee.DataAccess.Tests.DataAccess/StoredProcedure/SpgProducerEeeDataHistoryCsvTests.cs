namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
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
                var dataReturnVersion4 = helper.CreateDataReturnVersion(scheme2, 2000, 2);

                helper.CreateEeeOutputAmount(dataReturnVersion3, producer2.RegisteredProducer, "B2C", 1, 40);
                helper.CreateEeeOutputAmount(dataReturnVersion4, producer2.RegisteredProducer, "B2C", 2, 1000);

                db.Model.SaveChanges();

                // Act
                var results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN123");

                //Assert
                Assert.NotNull(results);

                ProducerEeeHistoryCsvData b2cProducer = results.Find(x => (x.ApprovalNumber == "WEE/TE0000S1/SCH"));
                Assert.NotNull(b2cProducer);
                Assert.Equal(2000, b2cProducer.ComplianceYear);
                Assert.Equal(1000, b2cProducer.Cat2B2C);
                Assert.Equal("Yes", b2cProducer.LatestData);

                ProducerEeeHistoryCsvData b2bProducer = results.Find(x => (x.ApprovalNumber == "WEE/TE0000ST/SCH"));
                Assert.NotNull(b2bProducer);
                Assert.Equal(2000, b2bProducer.ComplianceYear);
                Assert.Equal(200, b2bProducer.Cat2B2B);
                Assert.Null(b2bProducer.Cat2B2C);
                Assert.Equal("Yes", b2bProducer.LatestData);

                Assert.Equal(4, results.Count);                
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
                List<ProducerEeeHistoryCsvData> results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN345");

                //Assert
                Assert.NotNull(results);
                Assert.Equal("Yes", results[0].LatestData);
                Assert.Equal(600, results[0].Cat13B2B);

                Assert.Equal("No", results[1].LatestData);
                Assert.Equal(100, results[1].Cat10B2B);

                Assert.Equal("Yes", results[2].LatestData);
                Assert.Equal(200, results[2].Cat11B2B);

                Assert.Equal("Yes", results[3].LatestData);
                Assert.Equal(300, results[3].Cat12B2B);

                Assert.Equal("Yes", results[4].LatestData);
                Assert.Equal(500, results[4].Cat13B2B);

                Assert.Equal(5, results.Count);
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

                helper.CreateEeeOutputAmount(dataReturnVersion1, producer1.RegisteredProducer, "B2B", 1, 100);

                var scheme2 = helper.CreateScheme();
                scheme2.ApprovalNumber = "WEE/TE2345ST/SCH";
                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2000;

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "PRN567");
                producer2.ObligationType = "B2C";
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme2, 2000, 1);

                helper.CreateEeeOutputAmount(dataReturnVersion2, producer2.RegisteredProducer, "B2C", 1, 100);

                db.Model.SaveChanges();

                // Act
                List<ProducerEeeHistoryCsvData> results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN567");

                //Assert
                Assert.NotNull(results);
                Assert.Equal("Yes", results[0].LatestData);
                Assert.Equal("Yes", results[1].LatestData);
              
                Assert.Equal(2, results.Count);
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
                dataReturnVersion1.EeeOutputReturnVersion = version;
                dataReturnVersion2.EeeOutputReturnVersion = version;

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
                dataReturnVersion3.EeeOutputReturnVersion = version1;                

                db.Model.SaveChanges();

                // Act
                List<ProducerEeeHistoryCsvData> results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN897");

                //Assert
                Assert.NotNull(results);
               //Only shows entries for tonnage value changes and ignores the ones with no change.
                Assert.Equal(2, results.Count);
                Assert.Collection(results,
                   (r1) => Assert.Equal(new DateTime(2015, 1, 3), r1.SubmittedDate),
                   (r2) => Assert.Equal(new DateTime(2015, 1, 1), r2.SubmittedDate));
            }
        }

        [Fact]
        public async Task SpgProducerEeeDataHistoryCsvTests_EEEDataHistory_ReturnsProducerEeeDataHistoryOrderByCYDescending()
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
                List<ProducerEeeHistoryCsvData> results = await db.StoredProcedures.SpgProducerEeeHistoryCsvData("PRN111");

                //Assert
                Assert.NotNull(results);                
                Assert.Equal(5, results.Count);

                Assert.Collection(results,
                    (r1) => Assert.Equal(2005, r1.ComplianceYear),
                    (r2) => Assert.Equal(2004, r2.ComplianceYear),
                    (r3) => Assert.Equal(2001, r3.ComplianceYear),
                    (r4) => Assert.Equal(2000, r4.ComplianceYear),
                    (r5) => Assert.Equal(1999, r5.ComplianceYear));
            }
        }
    }
}
