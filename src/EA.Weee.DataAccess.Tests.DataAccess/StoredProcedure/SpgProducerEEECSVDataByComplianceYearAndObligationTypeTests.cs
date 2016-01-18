namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgProducerEEECSVDataByComplianceYearAndObligationTypeTests
    {
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
                    await db.StoredProcedures.SpgProducerEEECSVDataByComplianceYearAndObligationType(2000, "B2C");

                // Assert
                Assert.Equal(1, results.Count);

                var data = results.Single();

                Assert.Equal("PRN789", data.PRN);
                Assert.Equal(200, data.Cat1Q1);
            }
        }
    }
}
