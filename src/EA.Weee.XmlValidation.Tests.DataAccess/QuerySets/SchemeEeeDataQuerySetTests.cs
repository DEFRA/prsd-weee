namespace EA.Weee.XmlValidation.Tests.DataAccess.QuerySets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Obligation;
    using Weee.Tests.Core.Model;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using Xunit;

    public class SchemeEeeDataQuerySetTests
    {
        [Fact]
        public async Task HasProducerEeeDataForObligationType_ReturnsTrue_WhenDataAvailableForSpecifiedProducer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var schemeApprovalNumber = "WEE/AA0001YZ/SCH";

                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = schemeApprovalNumber;

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(schemeApprovalNumber, 2016, database.WeeeContext);
                var result = await querySet.HasProducerEeeDataForObligationType("WEE/MM0001AA", ObligationType.B2C);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task HasProducerEeeDataForObligationType_ReturnsFalse_WhenDataNotAvailableForSpecifiedProducer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var schemeApprovalNumber = "WEE/AA0001YZ/SCH";

                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = schemeApprovalNumber;

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(schemeApprovalNumber, 2016, database.WeeeContext);
                var result = await querySet.HasProducerEeeDataForObligationType("WEE/MM0001BB", ObligationType.B2C);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task HasProducerEeeDataForObligationType_ReturnsTrue_WhenDataAvailableForSpecifiedScheme()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/AA0001YZ/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet("WEE/AA0001YZ/SCH", 2016, database.WeeeContext);
                var result = await querySet.HasProducerEeeDataForObligationType("WEE/MM0001AA", ObligationType.B2C);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task HasProducerEeeDataForObligationType_ReturnsFalse_WhenDataNotAvailableForSpecifiedScheme()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/AA0001YZ/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet("WEE/AA0001AA/SCH", 2016, database.WeeeContext);
                var result = await querySet.HasProducerEeeDataForObligationType("WEE/MM0001AA", ObligationType.B2C);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task HasProducerEeeDataForObligationType_ReturnsTrue_WhenDataAvailableForSpecifiedComplianceYear()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/AA0001YZ/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet("WEE/AA0001YZ/SCH", 2016, database.WeeeContext);
                var result = await querySet.HasProducerEeeDataForObligationType("WEE/MM0001AA", ObligationType.B2C);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task HasProducerEeeDataForObligationType_ReturnsFalse_WhenDataNotAvailableForSpecifiedComplianceYear()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/AA0001YZ/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet("WEE/AA0001YZ/SCH", 2017, database.WeeeContext);
                var result = await querySet.HasProducerEeeDataForObligationType("WEE/MM0001AA", ObligationType.B2C);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task HasProducerEeeDataForObligationType_ReturnsTrue_WhenDataAvailableForSpecifiedObligationType()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/AA0001YZ/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet("WEE/AA0001YZ/SCH", 2016, database.WeeeContext);
                var result = await querySet.HasProducerEeeDataForObligationType("WEE/MM0001AA", ObligationType.B2C);

                // Assert
                Assert.True(result);
            }
        }

        [Fact]
        public async Task HasProducerEeeDataForObligationType_ReturnsFalse_WhenDataNotAvailableForSpecifiedObligationType()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();
                scheme.ApprovalNumber = "WEE/AA0001YZ/SCH";

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet("WEE/AA0001YZ/SCH", 2016, database.WeeeContext);
                var result = await querySet.HasProducerEeeDataForObligationType("WEE/MM0001AA", ObligationType.B2B);

                // Assert
                Assert.False(result);
            }
        }
    }
}
