namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Tests.Core.Model;
    using Xunit;

    public class SpgCSVDataByOrganisationIdAndComplianceYearTests
    {
        /// <summary>
        /// This test ensures that the "happy path" for the stored procedure results
        /// in a full dataset being populated for a single current producer.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithOneCurrentProducer_ReturnsFullSetOfData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;
                producer1.ChargeBandAmount = helper.FetchChargeBandAmount(ChargeBand.C);
                producer1.UpdatedDate = new DateTime(2015, 1, 1);

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCSVData result = results[0];

                Assert.Equal(producer1.Business.Company.Name, result.OrganisationName);
                Assert.Equal(producer1.TradingName, result.TradingName);
                Assert.Equal(producer1.RegistrationNumber, result.RegistrationNumber);
                Assert.Equal(producer1.Business.Company.CompanyNumber, result.CompanyNumber);
                Assert.Equal("C", result.ChargeBand);
                Assert.Equal(producer1.UpdatedDate, result.DateRegistered);
                Assert.Equal(producer1.UpdatedDate, result.DateAmended);
                Assert.Equal("No", result.AuthorisedRepresentative);
                Assert.Equal(string.Empty, result.OverseasProducer);
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

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

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

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);
                    
                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCSVData result = results[0];

                Assert.Equal(producer1.Business.Company.Name, result.OrganisationName);
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

                Producer producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);
                    
                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCSVData result = results[0];

                Assert.Equal(producer1.Business.Partnership.Name, result.OrganisationName);
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

                Producer producer1 = helper.CreateProducerAsSoleTrader(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCSVData result = results[0];

                Assert.Equal(string.Empty, result.OrganisationName);
            }
        }

        /// <summary>
        /// This test ensures that the registration date and amended date are correctly determined
        /// for a producer with multiple updates within the same compliance year.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithSeveralVersionsOfOneCurrentProducer_ReturnsLatestDataWithFirstRegistrationDate()
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
                producer1.IsCurrentForComplianceYear = false;
                producer1.UpdatedDate = new DateTime(2015, 1, 1);

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Producer producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA11");
                producer2.IsCurrentForComplianceYear = false;
                producer2.UpdatedDate = new DateTime(2015, 1, 2);

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.IsSubmitted = true;

                Producer producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");
                producer3.IsCurrentForComplianceYear = true;
                producer3.UpdatedDate = new DateTime(2015, 1, 3);

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCSVData result = results[0];

                Assert.Equal(producer3.RegistrationNumber, result.RegistrationNumber);
                Assert.Equal(producer1.UpdatedDate, result.DateRegistered);
                Assert.Equal(producer3.UpdatedDate, result.DateAmended);
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

                Producer producer2 = helper.CreateProducerAsPartnership(memberUpload2, "WEE/11AAAA11");
                producer2.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCSVData result = results[0];

                Assert.Equal(producer1.Business.Partnership.Name, result.OrganisationName);
            }
        }

        /// <summary>
        /// This test ensures that only producers registered in the specified scheme are returned.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithProducersInOtherSchemes_IgnoresOtherSchemes()
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
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCSVData result = results[0];

                Assert.Equal(producer1.RegistrationNumber, result.RegistrationNumber);
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

                Producer producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11BBBB11");
                producer1.IsCurrentForComplianceYear = true;
                producer1.Business.Partnership.Name = "BBBB";

                Producer producer2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/22AAAA22");
                producer2.IsCurrentForComplianceYear = true;
                producer2.Business.Company.Name = "AAAA";

                Producer producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.IsCurrentForComplianceYear = true;
                producer3.Business.Partnership.Name = "CCCC";

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(3, results.Count);

                Assert.Collection(results,
                    (r1) => Assert.Equal("AAAA", r1.OrganisationName),
                    (r2) => Assert.Equal("BBBB", r2.OrganisationName),
                    (r3) => Assert.Equal("CCCC", r3.OrganisationName));
            }
        }

        /// <summary>
        /// This test ensures that the single letter values are returned for the charge bands.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithDifferentChargeBandTypes_ReturnsChargeBandLetters()
        {
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
                    producer1.ChargeBandAmount = helper.FetchChargeBandAmount(ChargeBand.A);

                    Producer producer2 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/22BBBB22");
                    producer2.IsCurrentForComplianceYear = true;
                    producer2.ChargeBandAmount = helper.FetchChargeBandAmount(ChargeBand.B);

                    Producer producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                    producer3.IsCurrentForComplianceYear = true;
                    producer3.ChargeBandAmount = helper.FetchChargeBandAmount(ChargeBand.C);

                    Producer producer4 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/44DDDD44");
                    producer4.IsCurrentForComplianceYear = true;
                    producer4.ChargeBandAmount = helper.FetchChargeBandAmount(ChargeBand.D);

                    Producer producer5 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/55EEEE55");
                    producer5.IsCurrentForComplianceYear = true;
                    producer5.ChargeBandAmount = helper.FetchChargeBandAmount(ChargeBand.E);

                    db.Model.SaveChanges();

                    // Act
                    List<ProducerCSVData> results =
                        await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

                    // Assert
                    Assert.NotNull(results);
                    Assert.Equal(5, results.Count);

                    Assert.Collection(results,
                        (r1) => Assert.Equal("A", r1.ChargeBand),
                        (r2) => Assert.Equal("B", r2.ChargeBand),
                        (r3) => Assert.Equal("C", r3.ChargeBand),
                        (r4) => Assert.Equal("D", r4.ChargeBand),
                        (r5) => Assert.Equal("E", r5.ChargeBand));
                }
            }
        }

        /// <summary>
        /// This test ensures that the overseas producer name is returned for a producer with an
        /// authorised representative.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithAuthorisedRepresentative_ReturnsOverseasProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;

                AuthorisedRepresentative authorisedRepresentative = new AuthorisedRepresentative()
                {
                    Id = new Guid("620E71A6-0E74-47AF-B82F-97BA64083E37"),
                    OverseasProducerName = "Overseas Producer Name",
                };
                db.Model.AuthorisedRepresentatives.Add(authorisedRepresentative);

                producer1.AuthorisedRepresentative = authorisedRepresentative;

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2016);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCSVData result = results[0];

                Assert.Equal("No", result.AuthorisedRepresentative);
                Assert.Equal("Overseas Producer Name", result.OverseasProducer);
            }
        }

        /// <summary>
        /// This test ensures that the obligation type integer values are correctly mapped to
        /// there text equivalents. E.g. 1 = "B2B", 2 = "B2C", 3 = "Both'.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Execute_WithDifferentObligationTypes_MapsNumbersToObligationTypes()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2017;
                memberUpload1.IsSubmitted = true;

                Producer producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/11AAAA11");
                producer1.IsCurrentForComplianceYear = true;
                producer1.ObligationType = 1; // 1 = "B2B"

                Producer producer2 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/22BBBB22");
                producer2.IsCurrentForComplianceYear = true;
                producer2.ObligationType = 2; // 2 = "B2C"

                Producer producer3 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/33CCCC33");
                producer3.IsCurrentForComplianceYear = true;
                producer3.ObligationType = 3; // 3 = "Both"

                db.Model.SaveChanges();

                // Act
                List<ProducerCSVData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(scheme1.OrganisationId, 2017);

                // Assert
                Assert.NotNull(results);
                Assert.Equal(3, results.Count);

                Assert.Collection(results,
                    (r1) => Assert.Equal("B2B", r1.ObligationType),
                    (r2) => Assert.Equal("B2C", r2.ObligationType),
                    (r3) => Assert.Equal("Both", r3.ObligationType));
            }
        }
    }
}
