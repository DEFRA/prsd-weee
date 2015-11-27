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
                
                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");
                producer1.IsCurrentForComplianceYear = true;
                producer1.Business.Company.Contact1.Telephone = "55 5555 5555";
                producer1.Business.Company.Contact1.Email = "email@test.com";
                
                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                    await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                Assert.NotNull(results);
                Assert.True(results.TrueForAll(i => i.ComplianceYear == 2016));

                Assert.False(results.Any(i => i.PRN == "WEE/99ZZZZ99" && i.ComplianceYear == 2017));

                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99" && i.ComplianceYear == 2016);

                Assert.NotNull(result);
                Assert.Equal(result.CSROAAddress1, scheme1.Organisation.Address.Address1);
                Assert.Equal(result.CSROAPostcode, scheme1.Organisation.Address.Postcode);

                Assert.Equal(result.SchemeName, scheme1.SchemeName);
                Assert.Equal(result.SchemeOperator, scheme1.Organisation.Name);

                Assert.Equal(result.ROATelephone, producer1.Business.Company.Contact1.Telephone);
                Assert.Equal(result.ROAEmail, producer1.Business.Company.Contact1.Email);
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

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");
                producer1.IsCurrentForComplianceYear = true;

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

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "WEE/99ZZZZ99");
                producer1.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                   await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99");

                Assert.NotNull(result);
                Assert.Equal(result.ProducerName, producer1.Business.Company.Name);
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

                Producer producer1 = helper.CreateProducerAsPartnership(memberUpload1, "WEE/99ZZZZ99");
                producer1.IsCurrentForComplianceYear = true;

                db.Model.SaveChanges();

                // Act
                List<ProducerPublicRegisterCSVData> results =
                   await db.StoredProcedures.SpgProducerPublicRegisterCSVDataByComplianceYear(2016);

                // Assert
                var result = results.SingleOrDefault(i => i.PRN == "WEE/99ZZZZ99");

                Assert.NotNull(result);
                Assert.Equal(result.ProducerName, producer1.Business.Partnership.Name);
            }
        }
    }
}
