namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
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
                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme1, 2000, 2);

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

                Assert.Equal(results.Count, 4);                
            }
        }        
    }
}
