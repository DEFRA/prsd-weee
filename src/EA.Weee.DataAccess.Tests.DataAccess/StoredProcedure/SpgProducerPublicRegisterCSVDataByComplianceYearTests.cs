namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.DataAccess.StoredProcedure;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class SpgProducerPublicRegisterCSVDataByComplianceYearTests
    {
        [Fact]
        public async Task Execute_HappyPath_ProducerTypeIsCompany_PerfectAssociationOfSchemeToOrganisationAndReturnsSelectedComplianceYearRecords()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.SchemeName = "SchemeName";
                
                scheme1.Organisation.Name = "PCS operator name";
                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;
                
                ProducerSubmission producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");
                producerSubmission1.Business.Company.Contact1.Telephone = "55 5555 5555";
                producerSubmission1.Business.Company.Contact1.Email = "email@test.com";

                Scheme scheme2 = helper.CreateScheme();

                scheme2.OrganisationId = new Guid("84729d95-3c26-4c37-9730-f25fa2b6dbfc");
                scheme2.Organisation.Id = new Guid("84729d95-3c26-4c37-9730-f25fa2b6dbfc");
                scheme2.SchemeName = "A test scheme name";

                scheme2.Organisation.Name = "PCS operator name 1";
                scheme2.Organisation.BusinessAddressId = new Guid("ecdf60fc-1576-43e2-9bb4-e6c9dc9d721d");
                scheme2.Organisation.Address = helper.CreateOrganisationAddress();
                scheme2.Organisation.Address.Id = new Guid("ecdf60fc-1576-43e2-9bb4-e6c9dc9d721d");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2017;
                memberUpload2.IsSubmitted = true;

                ProducerSubmission producerSubmission2 = helper.CreateProducerAsCompany(memberUpload2, "WEE/11ZZZZ11");
                producerSubmission2.Business.Company.Contact1.Telephone = "55 5555 5555";
                producerSubmission2.Business.Company.Contact1.Email = "email@test.com";

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                    await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.TrueForAll(i => i.ComplianceYear == 2016));

                Assert.False(results.Any(i => i.PRN == "WEE/11ZZZZ11" && i.ComplianceYear == 2017));

                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99" && i.ComplianceYear == 2016);

                Assert.NotNull(result);
                Assert.Equal(result.CSROAAddress1, scheme1.Organisation.Address.Address1);
                Assert.Equal(result.CSROAPostcode, scheme1.Organisation.Address.Postcode);

                Assert.Equal(result.SchemeName, scheme1.SchemeName);
                Assert.Equal(result.SchemeOperator, scheme1.Organisation.Name);

                Assert.Equal(result.ROATelephone, producerSubmission1.Business.Company.Contact1.Telephone);
                Assert.Equal(result.ROAEmail, producerSubmission1.Business.Company.Contact1.Email);
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

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                
                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = false;

                ProducerSubmission producerSubmission = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                   await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                Assert.False(results.Any(i => i.PRN == "WEE/99ZZZZ99"));
            }
        }

        [Fact]
        public async Task Execute_ProducerTypeIsCompany_ReturnsCompanyNameAsProducerName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");

                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                ProducerSubmission producerSubmission = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                   await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99");

                Assert.NotNull(result);
                Assert.Equal(result.ProducerName, producerSubmission.Business.Company.Name);
            }
        }

        [Fact]
        public async Task Execute_ProducerTypeIsPartnership_ReturnsPartnershipNameAsProducerName()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(db.Model);

                Scheme scheme1 = helper.CreateScheme();

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");

                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                ProducerSubmission producerSubmission = helper.CreateProducerAsPartnership(memberUpload1, "WEE/99ZZZZ99");

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                   await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99");

                Assert.NotNull(result);
                Assert.Equal(result.ProducerName, producerSubmission.Business.Partnership.Name);
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

                scheme1.OrganisationId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.Organisation.Id = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
                scheme1.SchemeName = "Lamborghini";

                scheme1.Organisation.Name = "PCS operator name";
                scheme1.Organisation.BusinessAddressId = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");
                scheme1.Organisation.Address = helper.CreateOrganisationAddress();
                scheme1.Organisation.Address.Id = new Guid("b58e9cb2-b97e-4141-ad32-73c70284fc77");

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme1);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                ProducerSubmission producerSubmission1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");
                producerSubmission1.Business.Company.Name = "A company name";

                ProducerSubmission producerSubmission2 = helper.CreateProducerAsCompany(memberUpload1, "WEE/88ZZZZ88");
                producerSubmission2.Business.Company.Name = "B company name";

                Scheme scheme2 = helper.CreateScheme();

                scheme2.OrganisationId = new Guid("84729d95-3c26-4c37-9730-f25fa2b6dbfc");
                scheme2.Organisation.Id = new Guid("84729d95-3c26-4c37-9730-f25fa2b6dbfc");
                scheme2.SchemeName = "Aston Martin";

                scheme2.Organisation.Name = "PCS operator name 1";
                scheme2.Organisation.BusinessAddressId = new Guid("ecdf60fc-1576-43e2-9bb4-e6c9dc9d721d");
                scheme2.Organisation.Address = helper.CreateOrganisationAddress();
                scheme2.Organisation.Address.Id = new Guid("ecdf60fc-1576-43e2-9bb4-e6c9dc9d721d");

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme2);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                ProducerSubmission producerSubmission3 = helper.CreateProducerAsCompany(memberUpload2, "WEE/11ZZZZ11");
                producerSubmission3.Business.Company.Name = "C company name";

                ProducerSubmission producerSubmission4 = helper.CreateProducerAsCompany(memberUpload2, "WEE/22ZZZZ22");
                producerSubmission4.Business.Company.Name = "D company name";

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                    await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.FindIndex(i => i.SchemeName == "Aston Martin" && i.ProducerName == "C company name") < results.FindIndex(i => i.SchemeName == "Aston Martin" && i.ProducerName == "D company name"));
                Assert.True(results.FindIndex(i => i.SchemeName == "Lamborghini" && i.ProducerName == "A company name") < results.FindIndex(i => i.SchemeName == "Lamborghini" && i.ProducerName == "B company name"));
                Assert.True(results.FindIndex(i => i.SchemeName == "Aston Martin" && i.ProducerName == "C company name") < results.FindIndex(i => i.SchemeName == "Lamborghini" && i.ProducerName == "B company name"));
            }
        }
    }
}
