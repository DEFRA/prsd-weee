namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgProducerEeeCsvDataByComplianceYearAndObligationTypeTests
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
                var results = await db.StoredProcedures.SpgProducerEeeCsvDataByComplianceYearAndObligationType(2000, "B2C");

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
                    await db.StoredProcedures.SpgProducerEeeCsvDataByComplianceYearAndObligationType(2000, "B2C");

                // Assert
                Assert.Equal(1, results.Count);

                var data = results.Single();

                Assert.Equal("PRN789", data.PRN);
                Assert.Equal(200, data.Cat1Q1);
            }
        }
    }
}
