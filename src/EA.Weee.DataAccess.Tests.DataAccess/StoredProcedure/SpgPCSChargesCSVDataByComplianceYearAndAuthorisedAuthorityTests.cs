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

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results =
                    await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2016, scheme1.CompetentAuthorityId);

                // Assert
                Assert.NotNull(results);

                PCSChargesCSVData result = results.Find(x => (x.PRN == "WEE/11AAAA11"));

                Assert.Equal(producer1.RegistrationNumber, result.PRN);
                Assert.Equal(producer1.ChargeThisUpdate, result.ChargeValue);
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

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = false;

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");
                producer1.ChargeThisUpdate = 0;
                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2016);

                // Assert
                Assert.NotNull(results);
                PCSChargesCSVData result = results.Find(x => (x.PRN == "WEE/11AAAA11"));
                Assert.Null(result);
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

                Producer producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;

                Producer producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA12");
                producer2.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2016);
                // Assert
                Assert.NotNull(results);
              
                PCSChargesCSVData result1 = results.Find(x => (x.PRN == "WEE/11AAAA11"));

                Assert.Equal(producer1.Business.Partnership.Name, result1.ProducerName);

                PCSChargesCSVData result2 = results.Find(x => (x.PRN == "WEE/11AAAA12"));

                Assert.Null(result2);
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

                //EA Scheme
                var authorityId = new Guid("A3C2D0DD-53A1-4F6A-99D0-1CCFC87611A8");
                Scheme scheme1 = helper.CreateScheme();
                scheme1.CompetentAuthorityId = authorityId;

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                Producer producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;

                //SEPA Scheme
                var sepaId = new Guid("78F37814-364B-4FAE-BEB5-DB0439CBF177");
                Scheme scheme2 = helper.CreateScheme();
                scheme2.CompetentAuthorityId = sepaId;
                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Producer producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA12");
                producer2.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2016, authorityId);
                // Assert
                Assert.NotNull(results);

                PCSChargesCSVData result1 = results.Find(x => (x.PRN == "WEE/11AAAA11"));

                Assert.Equal(producer1.Business.Partnership.Name, result1.ProducerName);

                PCSChargesCSVData result2 = results.Find(x => (x.PRN == "WEE/11AAAA12"));

                Assert.Null(result2);
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

                Producer producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;

                Scheme scheme2 = helper.CreateScheme();

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Producer producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/22BBBB22");
                producer2.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2016);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(producer1.RegistrationNumber, results.SingleOrDefault(p => p.PRN == producer1.RegistrationNumber).PRN);
                Assert.Equal(producer2.RegistrationNumber, results.SingleOrDefault(p => p.PRN == producer2.RegistrationNumber).PRN);
            }
        }

        /// <summary>
        /// This test ensures that the results are ordered by organisation name.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralProducers_ReturnsResultsOrderedBySchemeNameAndSubmissionDate()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2020;
                memberUpload1.IsSubmitted = true;
                memberUpload1.Date = new DateTime(2015, 11, 4);
                Producer producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.IsCurrentForComplianceYear = true;
                producer1.Business.Partnership.Name = "ABCH";

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2020;
                memberUpload2.IsSubmitted = true;
                memberUpload2.Date = new DateTime(2015, 11, 1);
                Producer producer2 = helper.CreateProducerAsCompany(memberUpload2, "WEE/22AAAA22");
                producer2.IsCurrentForComplianceYear = true;
                producer2.Business.Company.Name = "AAAA";
                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2020;
                memberUpload3.IsSubmitted = true;
                memberUpload3.Date = new DateTime(2015, 10, 4);
                Producer producer3 = helper.CreateProducerAsPartnership(memberUpload3, "WEE/33CCCC33");
                producer3.IsCurrentForComplianceYear = true;
                producer3.Business.Partnership.Name = "ABCD";

                db.Model.SaveChanges();

                // Act
                List<PCSChargesCSVData> results = await db.StoredProcedures.SpgPCSChargesCSVDataByComplianceYearAndAuthorisedAuthority(2020);

                // Assert
                Assert.NotNull(results);

                Assert.Collection(results,
                    (r1) => Assert.Equal("04/10/2015", r1.SubmissionDate.ToShortDateString()),
                    (r2) => Assert.Equal("01/11/2015", r2.SubmissionDate.ToShortDateString()),
                    (r3) => Assert.Equal("04/11/2015", r3.SubmissionDate.ToShortDateString()));
            }
        }
    }
}
