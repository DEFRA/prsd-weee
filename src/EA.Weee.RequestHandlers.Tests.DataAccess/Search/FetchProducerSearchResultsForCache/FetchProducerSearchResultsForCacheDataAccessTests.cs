namespace EA.Weee.RequestHandlers.Tests.DataAccess.Search.FetchProducerSearchResultsForCache
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Weee.RequestHandlers.Search.FetchProducerSearchResultsForCache;
    using EA.Weee.Tests.Core.Model;
    using Xunit;
    using Model = EA.Weee.Tests.Core.Model;

    public class FetchProducerSearchResultsForCacheDataAccessTests
    {
        /// <summary>
        /// This test ensures that only the latest result will be returned for a producer who is
        /// registered in multiple compliance years.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchLatestProducers_WithProducerRegisteredIn2015And2016_ReturnsOnly2016Producer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Model.Scheme scheme = helper.CreateScheme();
                
                Model.MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Model.Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAA");
                producer1.IsCurrentForComplianceYear = true;

                Model.MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Model.Producer producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAA");
                producer2.IsCurrentForComplianceYear = true;

                database.Model.SaveChanges();

                // Act
                FetchProducerSearchResultsForCacheDataAccess dataAccess = new FetchProducerSearchResultsForCacheDataAccess(database.WeeeContext);
                var results = await dataAccess.FetchLatestProducers();

                // Assert
                Assert.NotNull(results);
                Assert.Contains(results, p => p.RegistrationNumber == "AAAAA" && p.Name == producer2.Business.Company.Name);
                Assert.DoesNotContain(results, p => p.RegistrationNumber == "AAAAA" && p.Name == producer1.Business.Company.Name);
            }
        }

        /// <summary>
        /// This test ensures that only the latest result will be returned for a producer who is
        /// registered multiple times in the same compliance year.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchLatestProducers_WithProducerRegisteredTwiceIn2015_ReturnsOnlyLatestProducer()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Model.Scheme scheme = helper.CreateScheme();

                Model.MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Model.Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAA");
                producer1.UpdatedDate = new DateTime(2015, 1, 1);
                producer1.IsCurrentForComplianceYear = false;

                Model.MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2015;
                memberUpload2.IsSubmitted = true;

                Model.Producer producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAA");
                producer2.UpdatedDate = new DateTime(2015, 1, 2);
                producer2.IsCurrentForComplianceYear = true;

                database.Model.SaveChanges();

                // Act
                FetchProducerSearchResultsForCacheDataAccess dataAccess = new FetchProducerSearchResultsForCacheDataAccess(database.WeeeContext);
                var results = await dataAccess.FetchLatestProducers();

                // Assert
                Assert.NotNull(results);
                Assert.Contains(results, p => p.RegistrationNumber == "AAAAA" && p.Name == producer2.Business.Company.Name);
                Assert.DoesNotContain(results, p => p.RegistrationNumber == "AAAAA" && p.Name == producer1.Business.Company.Name);
            }
        }

        /// <summary>
        /// This test ensures that only the results are ordered by registration number.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task FetchLatestProducers_WithThreeProducers_ReturnsProducersOrderedByRegistrationNumber()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                ModelHelper helper = new ModelHelper(database.Model);

                // Arrange
                Model.Scheme scheme = helper.CreateScheme();

                Model.MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Model.Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAA");
                producer1.IsCurrentForComplianceYear = true;

                Model.MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2015;
                memberUpload2.IsSubmitted = true;

                Model.Producer producer2 = helper.CreateProducerAsCompany(memberUpload2, "CCCCC");
                producer2.IsCurrentForComplianceYear = true;

                Model.MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2015;
                memberUpload3.IsSubmitted = true;

                Model.Producer producer3 = helper.CreateProducerAsCompany(memberUpload3, "BBBBB");
                producer3.IsCurrentForComplianceYear = true;

                database.Model.SaveChanges();

                // Act
                FetchProducerSearchResultsForCacheDataAccess dataAccess = new FetchProducerSearchResultsForCacheDataAccess(database.WeeeContext);
                var results = await dataAccess.FetchLatestProducers();

                // Assert
                Assert.NotNull(results);

                var result1 = results.SingleOrDefault(r => r.RegistrationNumber == "AAAAA");
                var result2 = results.SingleOrDefault(r => r.RegistrationNumber == "BBBBB");
                var result3 = results.SingleOrDefault(r => r.RegistrationNumber == "CCCCC");

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.NotNull(result3);

                int result1index = results.IndexOf(result1);
                int result2index = results.IndexOf(result2);
                int result3index = results.IndexOf(result3);

                Assert.True(result1index < result2index);
                Assert.True(result2index < result3index);
            }
        }
    }
}
