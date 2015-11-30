namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess
{
    using System;
    using FakeItEasy;
    using RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GenerateFromXmlDataAccessTests
    {
        [Theory]
        [InlineData("Australia", "DDC6551C-D9B2-465C-86DD-670B7D2142C2")]
        [InlineData("UK - England", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData("UK - Northern Ireland", "7BFB8717-4226-40F3-BC51-B16FDF42550C")]
        public async void GetCountry_ReturnsCorrectCountryForSpecifiedName(string countryName, string countryId)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetCountry(countryName);

                Assert.Equal(Guid.Parse(countryId), result.Id);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void GetCountry_WithNullOrEmptyCountryName_ReturnsNull(string countryName)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetCountry(countryName);

                Assert.Null(result);
            }
        }

        [Fact]
        public async void GetLatestProducerRecord_WithNoMatchingScheme_ReturnsNull()
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetLatestProducerRecord(Guid.NewGuid(), A<string>._);

                Assert.Null(result);
            }
        }

        [Fact]
        public async void GetLatestProducerRecord_WithNoMatchingProducerRegistrationNumber_ReturnsNull()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetLatestProducerRecord(scheme.Id, "XXXXXX");

                Assert.Null(result);
            }
        }

        [Fact]
        public async void GetLatestProducerRecord_WithMultipleRecord_SameProducerRegistrationNumber_ReturnsLatest()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.IsSubmitted = true;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "AA");
                producer1.UpdatedDate = new DateTime(2015, 1, 1); // 01/01/2015

                var producer2 = helper.CreateProducerAsCompany(memberUpload, "AA");
                producer2.UpdatedDate = new DateTime(2015, 2, 2, 10, 0, 0); // 10am 02/02/2015

                var producer3 = helper.CreateProducerAsCompany(memberUpload, "AA");
                producer3.UpdatedDate = new DateTime(2015, 2, 2, 9, 0, 0); // 9am 02/02/2015

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetLatestProducerRecord(scheme.Id, "AA");

                Assert.Equal(producer2.Id, result.Id);
            }
        }

        [Fact]
        public async void GetLatestProducerRecord_WithMultipleRecord_MultipleProducerRegistrationNumbers_ReturnsLatestForSpecifiedRegistrationNumber()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.IsSubmitted = true;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "BB");
                producer1.UpdatedDate = new DateTime(2015, 1, 1); // 01/01/2015

                var producer2 = helper.CreateProducerAsCompany(memberUpload, "AA");
                producer2.UpdatedDate = new DateTime(2015, 2, 2, 10, 0, 0); // 10am 02/02/2015

                var producer3 = helper.CreateProducerAsCompany(memberUpload, "BB");
                producer3.UpdatedDate = new DateTime(2015, 2, 2, 9, 0, 0); // 9am 02/02/2015

                var producer4 = helper.CreateProducerAsCompany(memberUpload, "AA");
                producer4.UpdatedDate = new DateTime(2016, 2, 2, 9, 0, 0); // 9am 02/02/2016

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetLatestProducerRecord(scheme.Id, "BB");

                Assert.Equal(producer3.Id, result.Id);
            }
        }

        [Fact]
        public async void GetLatestProducerRecord_WithUnSubmittedMemberUpload_ReturnsLatestForSubmittedUploadsOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.IsSubmitted = true;

                var producer1 = helper.CreateProducerAsCompany(memberUpload, "AA");
                producer1.UpdatedDate = new DateTime(2015, 1, 1); // 01/01/2015

                var producer2 = helper.CreateProducerAsCompany(memberUpload, "AA");
                producer2.UpdatedDate = new DateTime(2015, 2, 2, 10, 0, 0); // 10am 02/02/2015

                var producer3 = helper.CreateProducerAsCompany(memberUpload, "AA");
                producer3.UpdatedDate = new DateTime(2015, 2, 2, 9, 0, 0); // 9am 02/02/2015

                var memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.IsSubmitted = false;

                var producer4 = helper.CreateProducerAsCompany(memberUpload2, "AA");
                producer4.UpdatedDate = new DateTime(2016, 2, 2, 9, 0, 0); // 9am 02/02/2016

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetLatestProducerRecord(scheme.Id, "AA");

                Assert.Equal(producer2.Id, result.Id);
            }
        }

        [Fact]
        public async void GetLatestProducerRecord_ExcludeSpecifiedScheme_ReturnsLatestProducerFromOtherScheme()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme1 = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.IsSubmitted = true;

                var producer1 = helper.CreateProducerAsCompany(memberUpload1, "AA");
                producer1.UpdatedDate = new DateTime(2015, 1, 1); // 01/01/2015

                var producer2 = helper.CreateProducerAsCompany(memberUpload1, "AA");
                producer2.UpdatedDate = new DateTime(2015, 2, 2, 10, 0, 0); // 10am 02/02/2015

                var scheme2 = helper.CreateScheme();

                var memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.IsSubmitted = true;

                var producer3 = helper.CreateProducerAsCompany(memberUpload2, "AA");
                producer3.UpdatedDate = new DateTime(2015, 2, 2, 9, 0, 0); // 9am 02/02/2015

                var producer4 = helper.CreateProducerAsCompany(memberUpload2, "AA");
                producer4.UpdatedDate = new DateTime(2015, 2, 2, 6, 0, 0); // 6am 02/02/2015

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetLatestProducerRecordExcludeScheme(scheme1.Id, "AA");

                Assert.Equal(producer3.Id, result.Id);
            }
        }

        [Fact]
        public async void GetMigratedProducer_ForMatchingProducerRegistrationNumber()
        {
            using (var database = new DatabaseWrapper())
            {
                var migratedProducer = new MigratedProducer_();
                migratedProducer.CompanyNumber = "AAA";
                migratedProducer.ProducerName = "Test producer";
                migratedProducer.ProducerRegistrationNumber = "1234";

                database.Model.MigratedProducer_Set.Add(migratedProducer);
                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetMigratedProducer("1234");

                Assert.Equal(migratedProducer.Id, result.Id);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("xxxxx")]
        public async void GetMigratedProducer_ForUnknownOrNullProducerRegistrationNumber_ReturnsNull(string producerRegistrationNumber)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetMigratedProducer(producerRegistrationNumber);

                Assert.Null(result);
            }
        }
    }
}
