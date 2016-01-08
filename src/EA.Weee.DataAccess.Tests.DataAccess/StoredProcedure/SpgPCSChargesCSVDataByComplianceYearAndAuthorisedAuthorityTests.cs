namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthorityTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsProducerWithSelectedAA()
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
                producer1.ChargeThisUpdate = 999;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results =
                    await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2016, scheme1.CompetentAuthorityId);

                // Assert
                Assert.NotNull(results);

                PCSChargesCSVData result = results.Find(x => (x.PRN == "WEE/11AAAA11"));

                Assert.Equal("WEE/11AAAA11", result.PRN);
                Assert.Equal(999, result.ChargeValue);
            }
        }

        /// <summary>
        /// This test ensures that a producer in the database associated with a
        /// member upload that has not incurred any charge will not appear in the results.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSubmittedMemberUpload_IgnoresProducerWithNoCharge()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                // Use a year for which there is unlikely to be any data stored for that year in the database.
                int year = 2000;

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = year;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");
                producer1.ChargeThisUpdate = 0;
                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(year);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(0, results.Count);
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
                producer1.ChargeThisUpdate = 999;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA12");
                producer2.ChargeThisUpdate = 999;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2016);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.Any(r => r.PRN == "WEE/11AAAA11"), "Producers registered in the year specified should be returned.");
                Assert.False(results.Any(r => r.PRN == "WEE/11AAAA12"), "Producers not registered in the year specified should not be returned.");
            }
        }

        /// <summary>
        /// This test ensures the correct data is returned for the specified authorised authority
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentInSeveralYearsProducer_ReturnsTheCorrectYearandAAData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();
                scheme1.CompetentAuthorityId = new Guid("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");
                producer1.ChargeThisUpdate = 999;

                Scheme scheme2 = helper.CreateScheme();
                scheme2.CompetentAuthorityId = new Guid("78F37814-364B-4FAE-BEB5-DB0439CBF177");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA12");
                producer2.ChargeThisUpdate = 999;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(
                    2016,
                    new Guid("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8"));
                
                // Assert
                Assert.NotNull(results);
                Assert.True(results.Any(r => r.PRN == "WEE/11AAAA11"), "Producers registered in the specified year for a scheme with the specified AA should be returned.");
                Assert.False(results.Any(r => r.PRN == "WEE/11AAAA12"), "Producers registered in the specified year for a scheme with an AA other than the one specified should not be returned.");
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
                producer1.ChargeThisUpdate = 999;

                Scheme scheme2 = helper.CreateScheme();

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/22BBBB22");
                producer2.ChargeThisUpdate = 999;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2016);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.Any(r => r.PRN == "WEE/11AAAA11"), "Producers in all schemes should be returned when no scheme ID is specified.");
                Assert.True(results.Any(r => r.PRN == "WEE/22BBBB22"), "Producers in all schemes should be returned when no scheme ID is specified.");
            }
        }

        /// <summary>
        /// This test ensures that the results are ordered by the date upon which the member upload
        /// was uploaded.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralProducers_ReturnsResultsOrderedByDateOfMemberUpload()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2005;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 11, 4);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.ChargeThisUpdate = 999;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2005;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 11, 1);

                ProducerSubmission producer2 = helper.CreateProducerAsCompany(memberUpload2, "WEE/22AAAA22");
                producer2.ChargeThisUpdate = 999;

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2005;
                memberUpload3.IsSubmitted = true;
                memberUpload3.SubmittedDate = new DateTime(2015, 10, 4);

                ProducerSubmission producer3 = helper.CreateProducerAsPartnership(memberUpload3, "WEE/33CCCC33");
                producer3.ChargeThisUpdate = 999;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2005);

                // Assert
                Assert.NotNull(results);

                Assert.Collection(results,
                    (r1) => Assert.Equal(new DateTime(2015, 10, 4), r1.SubmissionDate),
                    (r2) => Assert.Equal(new DateTime(2015, 11, 1), r2.SubmissionDate),
                    (r3) => Assert.Equal(new DateTime(2015, 11, 4), r3.SubmissionDate));
            }
        }

        /// <summary>
        /// This test makes sure that all the charges history is returned for the producer
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSchemeHavingMultipleUploadsForProducer_ReturnsTheCorrectChargesforAllUploads()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();
                scheme1.CompetentAuthorityId = new Guid("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");
                producer1.ChargeThisUpdate = 60;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA11");
                producer2.ChargeThisUpdate = 30;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(
                    2016,
                    new Guid("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8"));

                // Assert
                Assert.True(results.Any(r => r.PRN == "WEE/11AAAA11" && r.ChargeValue == 60), "The charge of 60 should have been returned.");
                Assert.True(results.Any(r => r.PRN == "WEE/11AAAA11" && r.ChargeValue == 30), "The charge of 30 should have been returned.");
            }
        }
    }
}