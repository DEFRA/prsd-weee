namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberUploadTesting
{
    using System;
    using System.Linq;
    using RequestHandlers.Scheme.MemberUploadTesting;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class ProducerListFactoryDataAccessTests
    {
        [Fact]
        public async void FetchSchemeInfo_WithValidOrganisationId_ReturnsScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                Guid organisationId = Guid.NewGuid();
                ModelHelper modelHelper = new ModelHelper(database.Model);

                var scheme = modelHelper.CreateScheme();
                scheme.ApprovalNumber = "1234";
                scheme.Organisation.Id = organisationId;
                scheme.Organisation.TradingName = "TradingName";

                database.Model.SaveChanges();

                ProducerListFactoryDataAccess dataAccess = new ProducerListFactoryDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.FetchSchemeInfo(organisationId);

                // Assert
                Assert.Equal("1234", result.First().ApprovalNumber);
                Assert.Equal("TradingName", result.First().TradingName);
            }
        }

        [Fact]
        public async void GetRegistrationNumbers_ReturnsRegistrationNumbers()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                int complianceYear = 2015;

                Guid organisationId = Guid.NewGuid();
                var scheme = modelHelper.CreateScheme();
                scheme.Organisation.Id = organisationId;

                var memberUpload1 = modelHelper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = complianceYear;
                memberUpload1.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme, complianceYear, "1234");
                modelHelper.CreateProducerAsCompany(memberUpload1, "1234");

                var memberUpload2 = modelHelper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = complianceYear;
                memberUpload2.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme, complianceYear, "987");
                modelHelper.CreateProducerAsCompany(memberUpload2, "987");

                database.Model.SaveChanges();

                ProducerListFactoryDataAccess dataAccess = new ProducerListFactoryDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetRegistrationNumbers(organisationId, complianceYear, 2);

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains("1234", result);
                Assert.Contains("987", result);
            }
        }

        [Fact]
        public async void GetRegistrationNumbers_ReturnsSpecifiedNumberOfRegistrationNumbers()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                int complianceYear = 2015;

                Guid organisationId = Guid.NewGuid();
                var scheme = modelHelper.CreateScheme();
                scheme.Organisation.Id = organisationId;

                var memberUpload1 = modelHelper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = complianceYear;
                memberUpload1.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme, complianceYear, "1234");
                modelHelper.CreateProducerAsCompany(memberUpload1, "1234");

                var memberUpload2 = modelHelper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = complianceYear;
                memberUpload2.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme, complianceYear, "987");
                modelHelper.CreateProducerAsCompany(memberUpload2, "987");

                database.Model.SaveChanges();

                ProducerListFactoryDataAccess dataAccess = new ProducerListFactoryDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetRegistrationNumbers(organisationId, complianceYear, 1);

                // Assert
                Assert.Equal(1, result.Count);
            }
        }

        [Fact]
        public async void GetRegistrationNumbers_ReturnsRegistrationNumbers_ForSpecifiedOrganisationOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                int complianceYear = 2015;

                Guid organisationId1 = Guid.NewGuid();
                var scheme1 = modelHelper.CreateScheme();
                scheme1.Organisation.Id = organisationId1;

                var memberUpload1 = modelHelper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = complianceYear;
                memberUpload1.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme1, complianceYear, "1234");
                modelHelper.CreateProducerAsCompany(memberUpload1, "1234");

                Guid organisationId2 = Guid.NewGuid();
                var scheme2 = modelHelper.CreateScheme();
                scheme2.Organisation.Id = organisationId2;

                var memberUpload2 = modelHelper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = complianceYear;
                memberUpload2.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme2, complianceYear, "987");
                modelHelper.CreateProducerAsCompany(memberUpload2, "987");

                database.Model.SaveChanges();

                ProducerListFactoryDataAccess dataAccess = new ProducerListFactoryDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetRegistrationNumbers(organisationId2, complianceYear, 2);

                // Assert
                Assert.Equal(1, result.Count);
                Assert.Equal("987", result.First());
            }
        }

        [Fact]
        public async void GetRegistrationNumbers_ReturnsRegistrationNumbers_ForSpecifiedComplianceYearOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                Guid organisationId = Guid.NewGuid();
                var scheme = modelHelper.CreateScheme();
                scheme.Organisation.Id = organisationId;

                var memberUpload1 = modelHelper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme, 2015, "1234");
                modelHelper.CreateProducerAsCompany(memberUpload1, "1234");

                var memberUpload2 = modelHelper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme, 2016, "987");
                modelHelper.CreateProducerAsCompany(memberUpload2, "987");

                database.Model.SaveChanges();

                ProducerListFactoryDataAccess dataAccess = new ProducerListFactoryDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetRegistrationNumbers(organisationId, 2015, 2);

                // Assert
                Assert.Equal(1, result.Count);
                Assert.Equal("1234", result.First());
            }
        }

        [Fact]
        public async void GetRegistrationNumbers_ReturnsRegistrationNumbers_ForRegisteredProducerWithNonNullCurrentSubmissionOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper modelHelper = new ModelHelper(database.Model);

                int complianceYear = 2015;

                Guid organisationId = Guid.NewGuid();
                var scheme = modelHelper.CreateScheme();
                scheme.Organisation.Id = organisationId;

                var memberUpload1 = modelHelper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = complianceYear;
                memberUpload1.IsSubmitted = false;

                modelHelper.GerOrCreateRegisteredProducer(scheme, complianceYear, "1234");
                modelHelper.CreateProducerAsCompany(memberUpload1, "1234");

                var memberUpload2 = modelHelper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = complianceYear;
                memberUpload2.IsSubmitted = true;

                modelHelper.GerOrCreateRegisteredProducer(scheme, complianceYear, "987");
                modelHelper.CreateProducerAsCompany(memberUpload2, "987");

                database.Model.SaveChanges();

                ProducerListFactoryDataAccess dataAccess = new ProducerListFactoryDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.GetRegistrationNumbers(organisationId, complianceYear, 2);

                // Assert
                Assert.Equal(1, result.Count);
                Assert.Equal("987", result.First());
            }
        }
    }
}
