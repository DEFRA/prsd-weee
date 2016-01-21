namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthorityTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsProducerWithSelectedSchemeandAA()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.CompetentAuthorityId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results =
                    await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false,
                        scheme1.Id, scheme1.CompetentAuthorityId);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                MembersDetailsCSVData result = results[0];

                Assert.Equal("WEE/11AAAA11", result.PRN);
            }
        }

        /// <summary>
        /// This test ensures that a producer in the database associated with a
        /// member upload that has not yet been submitted will not appear in the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithNonSubmittedMemberUpload_IgnoresProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = false;

                helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, scheme1.Id, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(0, results.Count);
            }
        }

        /// <summary>
        /// This test ensures that for a producer which is a company, the company name
        /// is returned as the OrganisationName property of the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentProducerAsCompany_ReturnsCompanyName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, scheme1.Id, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                MembersDetailsCSVData result = results[0];

                Assert.Equal(producer1.Business.Company.Name, result.ProducerName);
            }
        }

        /// <summary>
        /// This test ensures that for a producer which is a partnership, the partnership name
        /// is returned as the OrganisationName property of the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentProducerAsPartnership_ReturnsPartnershipName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, scheme1.Id, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                MembersDetailsCSVData result = results[0];

                Assert.Equal(producer1.Business.Partnership.Name, result.ProducerName);
            }
        }

        /// <summary>
        /// This test ensures that for a producer which is a sole trader, an empty string
        /// is returned as the OrganisationName property of the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentProducerAsSoleTrader_ReturnsNoName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsSoleTrader(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, scheme1.Id, null);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                MembersDetailsCSVData result = results[0];

                Assert.Null(result.ProducerName);
            }
        }

        /// <summary>
        /// This test ensures that only producers registered in the specified compliance year
        /// are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentInSeveralYearsProducer_ReturnsTheCorrectYearsData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, scheme1.Id, null);
                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                MembersDetailsCSVData result = results[0];

                Assert.Equal(producer1.Business.Partnership.Name, result.ProducerName);
            }
        }

        /// <summary>
        /// This test ensures that only producers registered in the specified scheme are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithProducersInOtherSchemes_ReturnsOtherSchemesProducers()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");

                Scheme scheme2 = helper.CreateScheme();

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/22BBBB22");

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.Any(r => r.PRN == "WEE/11AAAA11"), "Producers from both schemes should be returned when no scheme ID is specified.");
                Assert.True(results.Any(r => r.PRN == "WEE/22BBBB22"), "Producers from both schemes should be returned when no scheme ID is specified.");
            }
        }

        /// <summary>
        /// This test ensures that the results are ordered by organisation name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralProducers_ReturnsResultsOrderedByOrganisationName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";

                ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";

                ProducerSubmission producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, scheme1.Id, null);

                // Assert
                Assert.NotNull(results);

                Assert.Collection(results,
                    (r1) => Assert.Equal("AAAA", r1.ProducerName),
                    (r2) => Assert.Equal("ABCD", r2.ProducerName),
                    (r3) => Assert.Equal("ABCH", r3.ProducerName));
            }
        }

        /// <summary>
        /// This test ensures that the results contains removed producers
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralProducersAndIncludeRemovedProducersIsYes_ReturnsAllProducersWithRemovedProducers()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";
                producer1.RegisteredProducer.Removed = true;

                ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";
                producer2.RegisteredProducer.Removed = true;

                ProducerSubmission producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, true, scheme1.Id, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(results.Count, 3);
                Assert.Collection(results,
                    (r1) => Assert.Equal("WEE/22AAAA22", r1.PRN),
                    (r2) => Assert.Equal("WEE/33CCCC33", r2.PRN),
                    (r3) => Assert.Equal("WEE/11BBBB11", r3.PRN));
            }
        }

        /// <summary>
        /// This test ensures that the results contains only non removed producers
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralProducersAndIncludeRemovedProducersIsNo_ReturnsAllProducersWithoutRemovedProducers()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.Business.Partnership.Name = "ABCH";
                producer1.RegisteredProducer.Removed = true;

                ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.Business.Company.Name = "AAAA";
                producer2.RegisteredProducer.Removed = true;

                ProducerSubmission producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.Business.Partnership.Name = "ABCD";
                producer3.RegisteredProducer.Removed = false;

                db.Model.SaveChanges();

                // Act
                List<MembersDetailsCSVData> results = await db.StoredProcedures.SpgCSVDataBySchemeComplianceYearAndAuthorisedAuthority(2016, false, scheme1.Id, null);

                // Assert
                Assert.NotNull(results);

                Assert.Equal(results.Count, 1);
                Assert.Collection(results,
                   (r1) => Assert.Equal("WEE/33CCCC33", r1.PRN));
            }
        }
    }
}
