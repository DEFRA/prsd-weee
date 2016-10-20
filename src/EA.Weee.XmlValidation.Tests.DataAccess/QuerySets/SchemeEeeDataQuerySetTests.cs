namespace EA.Weee.XmlValidation.Tests.DataAccess.QuerySets
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core.Model;
    using XmlValidation.BusinessValidation.MemberRegistration.QuerySets;
    using Xunit;

    public class SchemeEeeDataQuerySetTests
    {
        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsData_WhenDataAvailableForSpecifiedProducer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(organisation.Id, "2016", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001AA");

                // Assert
                Assert.NotNull(result);
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsNull_WhenDataNotAvailableForSpecifiedProducer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(organisation.Id, "2016", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001BB");

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsData_WhenDataAvailableForSpecifiedScheme()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(organisation.Id, "2016", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001AA");

                // Assert
                Assert.NotNull(result);
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsNull_WhenDataNotAvailableForSpecifiedScheme()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(Guid.NewGuid(), "2016", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001AA");

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsData_WhenDataAvailableForSpecifiedComplianceYear()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(organisation.Id, "2016", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001AA");

                // Assert
                Assert.NotNull(result);
                Assert.Single(result);
            }
        }

        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsNull_WhenDataNotAvailableForSpecifiedComplianceYear()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");
                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(organisation.Id, "2017", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001AA");

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsLatestDataOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");

                var dataReturn = helper.CreateDataReturn(scheme, 2016, 1);

                var dataReturnVersion1 = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true, dataReturn: dataReturn);
                var producerEee1 = helper.CreateEeeOutputAmount(dataReturnVersion1, producer.RegisteredProducer, "B2C", 1, 1000);

                var dataReturnVersion2 = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true, dataReturn: dataReturn);
                var producerEee2 = helper.CreateEeeOutputAmount(dataReturnVersion2, producer.RegisteredProducer, "B2C", 1, 2000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(organisation.Id, "2016", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001AA");

                // Assert
                Assert.NotNull(result);
                Assert.Single(result);
                Assert.Equal(2000, result.Single().Tonnage);
            }
        }

        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsSubmittedDataOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");

                var dataReturn = helper.CreateDataReturn(scheme, 2016, 1);

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: false, dataReturn: dataReturn);
                var producerEee = helper.CreateEeeOutputAmount(dataReturnVersion, producer.RegisteredProducer, "B2C", 1, 1000);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(organisation.Id, "2016", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001AA");

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetLatestProducerEeeData_ReturnsSubmittedDataWithEeeValuesOnly()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                var helper = new ModelHelper(database.Model);

                var organisation = helper.CreateOrganisation();
                var scheme = helper.CreateScheme(organisation);

                var memberUpload = helper.CreateSubmittedMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;

                var producer = helper.CreateProducerAsCompany(memberUpload, "WEE/MM0001AA");

                var dataReturn = helper.CreateDataReturn(scheme, 2016, 1);

                var dataReturnVersion = helper.CreateDataReturnVersion(scheme, 2016, 1, isSubmitted: true, dataReturn: dataReturn);

                database.Model.SaveChanges();

                // Act
                var querySet = new SchemeEeeDataQuerySet(organisation.Id, "2016", database.WeeeContext);
                var result = await querySet.GetLatestProducerEeeData("WEE/MM0001AA");

                // Assert
                Assert.Null(result);
            }
        }
    }
}
