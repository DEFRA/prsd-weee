namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
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
                
                Assert.Equal(results.Count, 14);
                
                var firstCategoryRecord = results[0];
                var secondCategoryRecord = results[1];
                
                Assert.Equal("1. Large Household Appliances", firstCategoryRecord.Category);
                Assert.Equal("2. Small Household Appliances", secondCategoryRecord.Category);

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
                Assert.Equal(results.Count, 14);
                var firstCategoryRecord = results[0];
                Assert.Equal("1. Large Household Appliances", firstCategoryRecord.Category);

                Assert.Null(firstCategoryRecord.Q3B2CEEE);
                Assert.Equal(700, firstCategoryRecord.TotalB2CEEE);
            }
        }
    }
}
