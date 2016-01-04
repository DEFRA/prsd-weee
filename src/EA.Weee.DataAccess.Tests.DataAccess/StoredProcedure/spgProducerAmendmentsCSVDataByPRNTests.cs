namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgProducerAmendmentsCSVDataByPRNTests
    {
        [Fact]
        public async Task Execute_HappyPath_ReturnsParticularPRNRecords()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.SchemeName = "SchemeName";

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");
                ProducerSubmission producerSubmission2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/11ZZZZ11");
                producerSubmission2.VATRegistered = true;

                Scheme scheme2 = helper.CreateScheme();

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producerSubmission3 = helper.CreateProducerAsCompany(memberUpload2, "WEE/11ZZZZ11");
                producerSubmission2.VATRegistered = false;

                db.Model.SaveChanges();

                // Act
                List<ProducerAmendmentsHistoryCSVData> results =
                    await db.StoredProcedures.SpgProducerAmendmentsCSVDataByPRN("WEE/11ZZZZ11");

                // Assert
                Assert.NotNull(results);
                Assert.Equal(results.Count, 2);
                Assert.True(results.TrueForAll(i => i.PRN == "WEE/11ZZZZ11"));
                Assert.False(results.TrueForAll(i => i.VATRegistered));
            }
        }

        [Fact]
        public async Task Execute_MultipleTimeProducerAmendmentsDuringComplianceYear_ReturnsAllAmendmentsRecordsWithPerfectRegisteredDateAndUpdatedDate()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.SchemeName = "SchemeName";

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 2);

                ProducerSubmission producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, "WEE/99ZZZZ99");

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.IsSubmitted = true;
                memberUpload3.SubmittedDate = new DateTime(2015, 1, 3);

                ProducerSubmission producerSubmission3 = helper.CreateProducerAsCompany(memberUpload3, "WEE/99ZZZZ99");

                db.Model.SaveChanges();

                // Act
                List<ProducerAmendmentsHistoryCSVData> results =
                    await db.StoredProcedures.SpgProducerAmendmentsCSVDataByPRN("WEE/99ZZZZ99");

                // Assert
                Assert.NotNull(results);
                Assert.Equal(results.Count, 3);

                results = results.OrderBy(i => i.DateAmended).ToList();

                Assert.Equal(new DateTime(2015, 1, 1), results[0].DateRegistered);
                Assert.Equal(new DateTime(2015, 1, 1), results[1].DateRegistered);
                Assert.Equal(new DateTime(2015, 1, 1), results[2].DateRegistered);

                Assert.Equal(new DateTime(2015, 1, 1), results[0].DateAmended);
                Assert.Equal(new DateTime(2015, 1, 2), results[1].DateAmended);
                Assert.Equal(new DateTime(2015, 1, 3), results[2].DateAmended);
            }
        }

        [Fact]
        public async Task Execute_NonSubmittedMemberUpload_IgnoresProducer()
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
                List<ProducerAmendmentsHistoryCSVData> results =
                    await db.StoredProcedures.SpgProducerAmendmentsCSVDataByPRN("WEE/11AAAA11");

                // Assert
                Assert.NotNull(results);
                Assert.Equal(0, results.Count);
            }
        }

        [Fact]
        public async Task Execute_ProducerAsCompany_ReturnsCompanyName()
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
                List<ProducerAmendmentsHistoryCSVData> results =
                    await db.StoredProcedures.SpgProducerAmendmentsCSVDataByPRN("WEE/11AAAA11");

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                
                Assert.Equal(producer1.Business.Company.Name, results[0].ProducerName);
            }
        }

        [Fact]
        public async Task Execute_ProducerAsPartnership_ReturnsPartnershipName()
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
                List<ProducerAmendmentsHistoryCSVData> results =
                    await db.StoredProcedures.SpgProducerAmendmentsCSVDataByPRN("WEE/11AAAA11");

                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);
                
                Assert.Equal(producer1.Business.Partnership.Name, results[0].ProducerName);
            }
        }

        [Fact]
        public async Task Execute_ListOfProducers_ReturnsRecordsInCurrectOrder()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.SchemeName = "SONY";

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2017;
                memberUpload1.IsSubmitted = true;
                memberUpload1.SubmittedDate = new DateTime(2015, 1, 1);

                ProducerSubmission producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme1);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;
                memberUpload2.SubmittedDate = new DateTime(2015, 1, 2);

                ProducerSubmission producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, "WEE/99ZZZZ99");

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme1);
                memberUpload3.ComplianceYear = 2017;
                memberUpload3.IsSubmitted = true;
                memberUpload3.SubmittedDate = new DateTime(2015, 1, 3);

                ProducerSubmission producerSubmission3 = helper.CreateProducerAsCompany(memberUpload3, "WEE/99ZZZZ99");
                
                MemberUpload memberUpload4 = helper.CreateMemberUpload(scheme1);
                memberUpload4.ComplianceYear = 2016;
                memberUpload4.IsSubmitted = true;
                memberUpload4.SubmittedDate = new DateTime(2015, 1, 4);

                ProducerSubmission producerSubmission4 = helper.CreateProducerAsCompany(memberUpload4, "WEE/99ZZZZ99");
                
                db.Model.SaveChanges();

                // Act
                List<ProducerAmendmentsHistoryCSVData> results =
                   await db.StoredProcedures.SpgProducerAmendmentsCSVDataByPRN("WEE/99ZZZZ99");

                // Assert
                Assert.NotNull(results);
                Assert.Equal(results.Count, 4);

                Assert.Equal(2017, results[0].ComplianceYear);
                Assert.Equal(new DateTime(2015, 1, 3), results[0].DateAmended);

                Assert.Equal(2017, results[1].ComplianceYear);
                Assert.Equal(new DateTime(2015, 1, 2), results[1].DateAmended);

                Assert.Equal(2017, results[2].ComplianceYear);
                Assert.Equal(new DateTime(2015, 1, 1), results[2].DateAmended);

                Assert.Equal(2016, results[3].ComplianceYear);
                Assert.Equal(new DateTime(2015, 1, 4), results[3].DateAmended);
            }
        }
    }
}
