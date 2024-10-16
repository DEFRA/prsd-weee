﻿namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess
{
    using EA.Weee.Domain.Producer;
    using EA.Weee.Domain.Producer.Classfication;
    using EA.Weee.Tests.Core;
    using RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GenerateFromXmlDataAccessTests
    {
        [Theory]
        [InlineData("Australia", "DDC6551C-D9B2-465C-86DD-670B7D2142C2")]
        [InlineData("UK - England", "184E1785-26B4-4AE4-80D3-AE319B103ACB")]
        [InlineData("UK - Northern Ireland", "7BFB8717-4226-40F3-BC51-B16FDF42550C")]
        public async Task GetCountry_ReturnsCorrectCountryForSpecifiedName(string countryName, string countryId)
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
        public async Task GetCountry_WithNullOrEmptyCountryName_ReturnsNull(string countryName)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.GetCountry(countryName);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task MigratedProducerExists_ForMatchingProducerRegistrationNumber_Returns_True()
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
                var result = await dataAccess.MigratedProducerExists("1234");

                Assert.True(result);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("xxxxx")]
        public async Task MigratedProducerExists_ForUnknownOrNullProducerRegistrationNumber_ReturnsFalse(string producerRegistrationNumber)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.MigratedProducerExists(producerRegistrationNumber);

                Assert.False(result);
            }
        }

        [Fact]
        public async Task ProducerRegistrationExists_ForMatchingProducerRegistrationNumber_ReturnsTrue()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;

                helper.CreateProducerAsCompany(memberUpload, "1234");

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.ProducerRegistrationExists("1234");

                Assert.True(result);
            }
        }

        [Fact]
        public async Task ProducerRegistrationExists_ForSmallMatchingProducerRegistrationNumber_ReturnsTrue()
        {
            using (var database = new DatabaseWrapper())
            {
                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "My company", "92345X", 2024);

                var submission1 = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, 2024, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.ProducerRegistrationExists("92345X");

                Assert.True(result);
            }
        }

        [Fact]
        public async Task ProducerRegistrationExists_ForMatchingProducerRegistrationNumber_WithoutProducerSubmission_ReturnsFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var registeredProducer = new Weee.Tests.Core.Model.RegisteredProducer
                {
                    Scheme = scheme,
                    ProducerRegistrationNumber = "1234"
                };

                database.Model.RegisteredProducers.Add(registeredProducer);
                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.ProducerRegistrationExists("1234");

                Assert.False(result);
            }
        }

        [Fact]
        public async Task ProducerRegistrationExists_ForMatchingSmallProducerRegistrationNumber_WithoutMatchingRegNumber_ReturnsFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var (_, directRegistrant, registeredProducer) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "My company", "12345X", 2024);

                var submission1 = await DirectRegistrantHelper.CreateSubmission(database, directRegistrant, registeredProducer, 2024, new List<DirectRegistrantHelper.EeeOutputAmountData>(), DirectProducerSubmissionStatus.Complete, SellingTechniqueType.Both.Value);

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.ProducerRegistrationExists("12345Z");

                Assert.False(result);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("xxxxx")]
        public async Task ProducerRegistrationExists_ForUnknownOrNullProducerRegistrationNumber_ReturnsFalse(string producerRegistrationNumber)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.ProducerRegistrationExists(producerRegistrationNumber);

                Assert.False(result);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("xxxxx")]
        public async Task ProducerRegistrationExists_ForUnknownOrNullSmallProducerRegistrationNumber_ReturnsFalse(string producerRegistrationNumber)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);

                var (_, directRegistrant, _) = DirectRegistrantHelper.CreateOrganisationWithRegisteredProducer(database, "My company", producerRegistrationNumber, 2024);

                database.WeeeContext.DirectRegistrants.Add(directRegistrant);
                await database.WeeeContext.SaveChangesAsync();

                var result = await dataAccess.ProducerRegistrationExists(producerRegistrationNumber);
                
                Assert.False(result);
            }
        }

        [Fact]
        public async Task SmallProducerRegistrationExists_ForMatchingProducerRegistrationNumber_ReturnsTrue()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2016;
                memberUpload.IsSubmitted = true;

                helper.CreateProducerAsCompany(memberUpload, "1234");

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.SchemeProducerRegistrationExists("1234");

                Assert.True(result);
            }
        }

        [Fact]
        public async Task SmallProducerRegistrationExists_ForMatchingProducerRegistrationNumber_WithoutProducerSubmission_ReturnsFalse()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var registeredProducer = new Weee.Tests.Core.Model.RegisteredProducer
                {
                    Scheme = scheme,
                    ProducerRegistrationNumber = "1234"
                };

                database.Model.RegisteredProducers.Add(registeredProducer);
                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.SchemeProducerRegistrationExists("1234");

                Assert.False(result);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("xxxxx")]
        public async Task SmallProducerRegistrationExists_ForUnknownOrNullProducerRegistrationNumber_ReturnsFalse(string producerRegistrationNumber)
        {
            using (var database = new DatabaseWrapper())
            {
                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.SchemeProducerRegistrationExists(producerRegistrationNumber);

                Assert.False(result);
            }
        }

        [Fact]
        public async Task FetchRegisteredProducerOrDefault_ReturnsProducerForSpecifiedComplianceYearOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.IsSubmitted = true;
                memberUpload1.ComplianceYear = 2015;

                var producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "1234");

                var memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.IsSubmitted = true;
                memberUpload2.ComplianceYear = 2016;

                var producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, "1234");

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.FetchRegisteredProducerOrDefault("1234", 2015, scheme.Id);

                Assert.Equal(producerSubmission1.Id, result.CurrentSubmission.Id);
                Assert.NotEqual(producerSubmission2.Id, result.CurrentSubmission.Id);
            }
        }

        [Fact]
        public async Task FetchRegisteredProducerOrDefault_ReturnsProducerForSpecifiedProducerRegistrationNumberOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.IsSubmitted = true;
                memberUpload1.ComplianceYear = 2015;

                var producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "6543");

                var memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.IsSubmitted = true;
                memberUpload2.ComplianceYear = 2015;

                var producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, "1234");

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.FetchRegisteredProducerOrDefault("1234", 2015, scheme.Id);

                Assert.NotEqual(producerSubmission1.Id, result.CurrentSubmission.Id);
                Assert.Equal(producerSubmission2.Id, result.CurrentSubmission.Id);
            }
        }

        [Fact]
        public async Task FetchRegisteredProducerOrDefault_ReturnsProducerForSpecifiedSchemeOnly()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                var memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.IsSubmitted = true;
                memberUpload1.ComplianceYear = 2015;

                var producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "1234");

                var scheme2 = helper.CreateScheme();

                var memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.IsSubmitted = true;
                memberUpload2.ComplianceYear = 2015;

                var producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, "1234");

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.FetchRegisteredProducerOrDefault("1234", 2015, scheme2.Id);

                Assert.NotEqual(producerSubmission1.Id, result.CurrentSubmission.Id);
                Assert.Equal(producerSubmission2.Id, result.CurrentSubmission.Id);
            }
        }

        [Fact]
        public async Task FetchRegisteredProducerOrDefault_WithNoCurrentSubmissionForProducer_ReturnsRegisteredProducerRecord()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);

                var scheme = helper.CreateScheme();

                helper.GetOrCreateRegisteredProducer(scheme, 2015, "1234");

                database.Model.SaveChanges();

                var dataAccess = new GenerateFromXmlDataAccess(database.WeeeContext);
                var result = await dataAccess.FetchRegisteredProducerOrDefault("1234", 2015, scheme.Id);

                Assert.Equal("1234", result.ProducerRegistrationNumber);
                Assert.Equal(2015, result.ComplianceYear);
                Assert.Equal(scheme.Id, result.Scheme.Id);
            }
        }
    }
}
