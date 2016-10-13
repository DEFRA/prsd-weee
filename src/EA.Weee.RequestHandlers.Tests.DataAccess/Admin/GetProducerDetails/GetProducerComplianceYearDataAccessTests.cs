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

    public class GetProducerComplianceYearDataAccessTests
    {
        [Fact]
        public async Task GetComplianceYears_ReturnsComplianceYearForRegisteredProducer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "AAA");

                database.Model.SaveChanges();

                var dataAccess = new GetProducerComplianceYearDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetComplianceYears("AAA");

                // Assert
                Assert.Single(result);
                Assert.Contains(2016, result);
            }
        }

        [Fact]
        public async Task GetComplianceYears_ReturnsComplianceYearForSpecifiedProducerRegistrationNumberOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "AAA");

                database.Model.SaveChanges();

                var dataAccess = new GetProducerComplianceYearDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetComplianceYears("BBB");

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetComplianceYears_DoesNotReturnUnsubmittedUploads()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = false;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "AAA");

                database.Model.SaveChanges();

                var dataAccess = new GetProducerComplianceYearDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetComplianceYears("AAA");

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetComplianceYears_ReturnsAllComplianceYearsForRegisteredProducer()
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

                var dataAccess = new GetProducerComplianceYearDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetComplianceYears("AAA");

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains(2016, result);
                Assert.Contains(2017, result);
            }
        }

        [Fact]
        public async Task GetComplianceYears_ProducerRegisteredWithTwoSchemesInSameYear_ReturnsOneInstanceOfComplianceYear()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                var scheme1 = helper.CreateScheme();
                var memberUpload1 = helper.CreateSubmittedMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAA");

                var scheme2 = helper.CreateScheme();
                var memberUpload2 = helper.CreateSubmittedMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAA");

                database.Model.SaveChanges();

                var dataAccess = new GetProducerComplianceYearDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetComplianceYears("AAA");

                // Assert
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetComplianceYears_ReturnsComplianceYearsInDescendingOrder()
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
                memberUpload2.ComplianceYear = 2018;
                var producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAA");

                var memberUpload3 = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2017;
                var producer3 = helper.CreateProducerAsCompany(memberUpload3, "AAA");

                database.Model.SaveChanges();

                var dataAccess = new GetProducerComplianceYearDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetComplianceYears("AAA");

                // Assert
                Assert.Equal(3, result.Count);
                Assert.Collection(result,
                    r1 => Assert.Equal(2018, r1),
                    r2 => Assert.Equal(2017, r2),
                    r3 => Assert.Equal(2016, r3));
            }
        }
    }
}