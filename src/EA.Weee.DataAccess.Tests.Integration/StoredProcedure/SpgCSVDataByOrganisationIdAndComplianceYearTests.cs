namespace EA.Weee.DataAccess.Tests.Integration.StoredProcedure
{
    using EA.Prsd.Core.Domain;
using EA.Weee.DataAccess.StoredProcedure;
using EA.Weee.DataAccess.Tests.Integration.Model;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

    public class SpgCSVDataByOrganisationIdAndComplianceYearTests
    {
        [Fact]
        public async Task SpgCSVDataByOrganisationIdAndComplianceYear_HappyPath()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                // Arrange
                Guid organisation1Id = new Guid("B578C1CF-A4E9-448C-91E9-FEC1BF754987");
                db.Model.Organisations.Add(new Organisation
                {
                    Id = organisation1Id,
                    TradingName = "Organisation 1 Trading Name",
                });

                Guid scheme1Id = new Guid("4641FBDC-B34B-4DB2-8CD6-F719540D6C92");
                db.Model.Schemes.Add(new Scheme
                {
                    Id = scheme1Id,
                    OrganisationId = organisation1Id,
                });

                Guid memberUpload1 = new Guid("BB3B2318-A4BB-4321-94DD-836898DE880E");
                db.Model.MemberUploads.Add(new MemberUpload
                {
                    Id = memberUpload1,
                    OrganisationId = organisation1Id,
                    SchemeId = scheme1Id,
                    ComplianceYear = 2016,
                    IsSubmitted = true,
                    Data = "<memberUpload1 />",
                });

                Guid address1Id = new Guid("56640071-E499-46A2-8F00-E11B0F08EC89");
                db.Model.Address1.Add(new Address1
                {
                    Id = address1Id,
                    PrimaryName = "Address 1 Primary Name",
                    SecondaryName = "Address 1 Secondary Name",
                    Street = "Address 1 Street",
                    Town = "Address 1 Town",
                    Locality = "Address 1 Locality",
                    AdministrativeArea = "Address 1 Admin Area",
                    PostCode = "Address 1 Post Code",
                    CountryId = new Guid("C2D3F176-48E5-42FC-B06B-1520173B7879"), // UK - England
                });

                Guid contactId1 = new Guid();
                db.Model.Contact1.Add(new Contact1
                {
                    Id = contactId1,
                    Title = "Contact 1 Title",
                    Forename = "Contact 1 Forename",
                    Surname = "Contact 1 Surname",
                    Telephone = "Contact 1 Telephone",
                    Mobile = "Contact 1 Mobile",
                    Fax = "Contact 1 Fax",
                    Email = "Contact 1 Email",
                    AddressId = address1Id,
                });

                Guid companyId1 = new Guid("56640071-E499-46A2-8F00-E11B0F08EC89");
                db.Model.Companies.Add(new Company
                {
                    Id = companyId1,
                    Name = "Company 1 Name",
                    CompanyNumber = "11111111",
                    RegisteredOfficeContactId = contactId1,
                });

                Guid businessId1 = new Guid();
                db.Model.Businesses.Add(new Business
                {
                    Id = businessId1,
                    CompanyId = companyId1,
                });

                Guid producerId1 = new Guid("94CF5223-A1DB-428E-B60E-BD240E19758E");
                db.Model.Producers.Add(new Producer
                {
                    Id = producerId1,
                    MemberUploadId = memberUpload1,
                    IsCurrentForComplianceYear = true,
                    RegistrationNumber = "WEE/11AAAA11",
                    TradingName = "Producer 1 Trading Name",
                    UpdatedDate = new DateTime(2015, 1, 1, 0, 0, 0),
                    ProducerBusinessId = businessId1,
                    SchemeId = scheme1Id,
                    ChargeBandType = 2, // 2 = "C",
                    AuthorisedRepresentativeId = null,
                });

                db.Model.SaveChanges();

                // Act
                List<ProducerCsvData> results =
                    await db.StoredProcedures.SpgCSVDataByOrganisationIdAndComplianceYear(organisation1Id, 2016);
                    
                // Assert
                Assert.NotNull(results);
                Assert.Equal(1, results.Count);

                ProducerCsvData producer1 = results[0];

                Assert.Equal("Company 1 Name", producer1.OrganisationName);
                Assert.Equal("Producer 1 Trading Name", producer1.TradingName);
                Assert.Equal("WEE/11AAAA11", producer1.RegistrationNumber);
                Assert.Equal("11111111", producer1.CompanyNumber);
                Assert.Equal("C", producer1.ChargeBand);
                Assert.Equal(new DateTime(2015, 1, 1, 0, 0, 0), producer1.DateRegistered);
                Assert.Equal(new DateTime(2015, 1, 1, 0, 0, 0), producer1.DateAmended);
                Assert.Equal("No", producer1.AuthorisedRepresentative);
                Assert.Equal(string.Empty, producer1.OverseasProducer);
            }
        }
    }
}
