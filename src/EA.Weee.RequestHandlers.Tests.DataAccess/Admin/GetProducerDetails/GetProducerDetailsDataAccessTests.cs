namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.GetProducerDetails
{
    using RequestHandlers.Admin.GetProducerDetails;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetProducerDetailsDataAccessTests
    {
        [Fact]
        public async Task Fetch_ReturnsDataWithMatchingProducerRegistrationNumberOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "AAA");
                var producer2 = helper.CreateProducerAsCompany(memberUpload, "BBB");

                database.Model.SaveChanges();

                var dataAccess = new GetProducerDetailsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.Fetch("AAA", 2016);

                // Assert
                Assert.Single(result);
                Assert.Equal("AAA", result.Single().RegisteredProducer.ProducerRegistrationNumber);
            }
        }

        [Fact]
        public async Task Fetch_ReturnsDataForSpecifiedComplianceYearOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAA");

                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2017;
                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAA");

                database.Model.SaveChanges();

                var dataAccess = new GetProducerDetailsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.Fetch("AAA", 2016);

                // Assert
                Assert.Single(result);
                Assert.Equal(2016, result.Single().RegisteredProducer.ComplianceYear);
            }
        }

        [Fact]
        public async Task Fetch_ReturnsDataForSubmittedProducerOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = false;

                var producer = helper.CreateProducerAsCompany(memberUpload, "AAA");

                database.Model.SaveChanges();

                var dataAccess = new GetProducerDetailsDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.Fetch("AAA", 2016);

                // Assert
                Assert.Empty(result);
            }
        }
    }
}
