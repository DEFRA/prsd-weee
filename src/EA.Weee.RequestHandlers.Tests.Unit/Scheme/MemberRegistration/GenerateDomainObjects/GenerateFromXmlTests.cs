namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration.GenerateDomainObjects
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using RequestHandlers.Scheme.Interfaces;
    using RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess;
    using RequestHandlers.Scheme.MemberRegistration.GenerateProducerObjects;
    using Requests.Scheme.MemberRegistration;
    using Xml.Converter;
    using Xml.MemberRegistration;
    using Xunit;
    using schemeType = Xml.MemberRegistration.schemeType;

    public class GenerateFromXmlTests
    {
        [Fact]
        public void GenerateMemberUpload_SchemaErrors_NullComplianceYear()
        {
            var message = new ProcessXMLFile(Guid.NewGuid(), new byte[1], "File name");

            var generateFromXml = new GenerateFromXmlBuilder().Build();

            var result = generateFromXml.GenerateMemberUpload(message,
                new List<MemberUploadError>
                {
                    new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Schema, "Some schema error")
                },
                A<decimal>._, A<Guid>._);

            Assert.Null(result.ComplianceYear);
        }

        [Fact]
        public void GenerateMemberUpload_NoSchemaErrors_ComplianceYearObtained()
        {
            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.XmlConverter.Deserialize(A<XDocument>._))
                .Returns(new schemeType { complianceYear = "2015" });

            var message = new ProcessXMLFile(Guid.NewGuid(), new byte[1], "File name");
            var generateFromXml = builder.Build();

            var result = generateFromXml.GenerateMemberUpload(message, new List<MemberUploadError>(), 2015, A<Guid>._);

            Assert.NotNull(result.ComplianceYear);
            Assert.Equal(2015, result.ComplianceYear.Value);
        }

        [Fact]
        public void GenerateMemberUpload_NullSchemaErrors_ComplianceYearObtained()
        {
            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.XmlConverter.Deserialize(A<XDocument>._))
                .Returns(new schemeType { complianceYear = "2015" });

            var message = new ProcessXMLFile(Guid.NewGuid(), new byte[1], "File name");
            var generateFromXml = builder.Build();

            var result = generateFromXml.GenerateMemberUpload(message, null, 2015, A<Guid>._);

            Assert.NotNull(result.ComplianceYear);
            Assert.Equal(2015, result.ComplianceYear.Value);
        }

        [Fact]
        public void GenerateMemberUpload_ReturnsNewMemberUpload_WithCorrectValues()
        {
            var message = new ProcessXMLFile(Guid.NewGuid(), new byte[1], "File name");
            decimal totalCharges = 1000M;
            var schemeId = Guid.NewGuid();
            var errors = new List<MemberUploadError>
                {
                    new MemberUploadError(ErrorLevel.Error, MemberUploadErrorType.Business, "Some schema error")
                };

            var builder = new GenerateFromXmlBuilder();

            string xml = "Test xml contents";
            A.CallTo(() => builder.XmlConverter.XmlToUtf8String(A<byte[]>._)).Returns(xml);

            schemeType scheme = new schemeType() { complianceYear = "2015" };
            A.CallTo(() => builder.XmlConverter.Deserialize(A<XDocument>._)).Returns(scheme);

            var result = builder.Build().GenerateMemberUpload(message, errors, totalCharges, schemeId);

            Assert.Equal(message.OrganisationId, result.OrganisationId);
            Assert.Equal(xml, result.RawData.Data);
            Assert.Equal(errors, result.Errors);
            Assert.Equal(totalCharges, result.TotalCharges);
            Assert.Equal(scheme.complianceYear, result.ComplianceYear.Value.ToString());
            Assert.Equal(schemeId, result.SchemeId);
            Assert.Equal(message.FileName, result.FileName);
        }
        
        [Theory]
        [InlineData(countryType.UKENGLAND, "UK - ENGLAND")]
        [InlineData(countryType.ISLEOFMAN, "ISLE OF MAN")]
        [InlineData(countryType.BELARUS, "BELARUS")]
        public void GetCountryName_RetrievesCountryNameFromAttributeIfPresent(countryType country, string countryName)
        {
            var builder = new GenerateFromXmlBuilder();
            var result = builder.Build().GetCountryName(country);

            Assert.Equal(countryName, result);
        }

        [Fact]
        public async void GetProducerContact_ReturnsContactWithCorrectAddressDetails()
        {
            string primary = "Primary";
            string secondary = "Secondary";
            string street = "Street";
            string locality = "Locality";
            string admimistrativeArea = "Area";
            string postCode = "WE3";
            countryType country = countryType.UKENGLAND;
            string countryName = "UK - ENGLAND";

            var contact = new contactDetailsType()
            {
                address = new addressType()
                {
                    primaryName = primary,
                    secondaryName = secondary,
                    streetName = street,
                    locality = locality,
                    administrativeArea = admimistrativeArea,
                    Item = postCode,
                    country = country
                }
            };

            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.DataAccess.GetCountry(A<string>._)).Returns(new Country(Guid.NewGuid(), countryName));

            var result = await builder.Build().GetProducerContact(contact);
            var resultAddress = result.Address;

            Assert.Equal(primary, resultAddress.PrimaryName);
            Assert.Equal(secondary, resultAddress.SecondaryName);
            Assert.Equal(street, resultAddress.Street);
            Assert.Equal(locality, resultAddress.Locality);
            Assert.Equal(admimistrativeArea, resultAddress.AdministrativeArea);
            Assert.Equal(postCode, resultAddress.PostCode);
            Assert.Equal(countryName, resultAddress.Country.Name);
        }

        [Fact]
        public async void GetProducerContact_ReturnsContactWithIndividualDetails()
        {
            string title = "title";
            string forename = "forename";
            string surname = "surname";
            string landline = "987654";
            string mobile = "12345";
            string fax = "564";
            string email = "a@b.c";

            var contact = new contactDetailsType()
            {
                address = new addressType(),
                title = title,
                forename = forename,
                surname = surname,
                phoneLandLine = landline,
                phoneMobile = mobile,
                fax = fax,
                email = email
            };

            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.DataAccess.GetCountry(A<string>._)).Returns(new Country(Guid.NewGuid(), email));

            var result = await builder.Build().GetProducerContact(contact);

            Assert.Equal(title, result.Title);
            Assert.Equal(forename, result.ForeName);
            Assert.Equal(surname, result.SurName);
            Assert.Equal(landline, result.Telephone);
            Assert.Equal(mobile, result.Mobile);
            Assert.Equal(fax, result.Fax);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async void SetProducerBusiness_WithCompanyBusiness_ReturnsBusinessWithCompanyDetails()
        {
            string companyName = "Company name";
            string companyRegistration = "CRN1234";
            string town = "Town";

            var business = new producerBusinessType()
            {
                correspondentForNotices = new optionalContactDetailsContainerType(),
                Item = new companyType()
                {
                    companyName = companyName,
                    companyNumber = companyRegistration,
                    registeredOffice = new contactDetailsContainerType()
                    {
                        contactDetails = new contactDetailsType()
                        {
                            address = new addressType()
                            {
                                town = town,
                                country = countryType.UKENGLAND
                            }
                        }
                    }
                }
            };

            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.DataAccess.GetCountry(A<string>._)).Returns((Country)null);

            var result = await builder.Build().SetProducerBusiness(business);

            Assert.NotNull(result.CompanyDetails);
            Assert.Null(result.Partnership);
            Assert.Equal(companyName, result.CompanyDetails.Name);
            Assert.Equal(companyRegistration, result.CompanyDetails.CompanyNumber);
            Assert.Equal(town, result.CompanyDetails.RegisteredOfficeContact.Address.Town);
        }

        [Fact]
        public async void SetProducerBusiness_WithPartnership_ReturnsBusinessWithPartnershipDetails()
        {
            var partnershipName = "Company name";
            var partners = new[] { "P1" };
            string town = "Town";

            var business = new producerBusinessType()
            {
                correspondentForNotices = new optionalContactDetailsContainerType(),
                Item = new partnershipType()
                {
                    partnershipName = partnershipName,
                    partnershipList = partners,
                    principalPlaceOfBusiness = new contactDetailsContainerType()
                    {
                        contactDetails = new contactDetailsType()
                        {
                            address = new addressType()
                            {
                                town = town,
                                country = countryType.UKENGLAND
                            }
                        }
                    }
                }
            };

            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.DataAccess.GetCountry(A<string>._)).Returns((Country)null);

            var result = await builder.Build().SetProducerBusiness(business);

            Assert.NotNull(result.Partnership);
            Assert.Null(result.CompanyDetails);
            Assert.Equal(partnershipName, result.Partnership.Name);
            Assert.Equal(town, result.Partnership.PrincipalPlaceOfBusiness.Address.Town);
        }

        [Fact]
        public async void SetProducerBusiness_WithPartnership_ReturnsPartnershipWithPartnersDetails()
        {
            var partners = new[] { "P1", "P3" };

            var business = new producerBusinessType()
            {
                correspondentForNotices = new optionalContactDetailsContainerType(),
                Item = new partnershipType()
                {
                    partnershipList = partners,
                    principalPlaceOfBusiness = new contactDetailsContainerType()
                    {
                        contactDetails = new contactDetailsType()
                        {
                            address = new addressType()
                            {
                                country = countryType.UKENGLAND
                            }
                        }
                    }
                }
            };

            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.DataAccess.GetCountry(A<string>._)).Returns((Country)null);

            var result = await builder.Build().SetProducerBusiness(business);

            Assert.Collection(result.Partnership.PartnersList,
                r1 => Assert.Equal("P1", r1.Name),
                r1 => Assert.Equal("P3", r1.Name));
        }

        [Fact]
        public async void SetProducerBusiness_WithCorrespondentForNotices_ReturnsCorrespondentForNoticesDetails()
        {
            string forename = "forename";
            string surname = "surname";

            var business = new producerBusinessType()
            {
                correspondentForNotices = new optionalContactDetailsContainerType()
                {
                    contactDetails = new contactDetailsType()
                    {
                        address = new addressType()
                        {
                            country = countryType.UKENGLAND
                        },
                        forename = forename,
                        surname = surname
                    }
                }
            };

            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.DataAccess.GetCountry(A<string>._)).Returns((Country)null);

            var result = await builder.Build().SetProducerBusiness(business);

            Assert.NotNull(result.CorrespondentForNoticesContact);
            Assert.Equal(forename, result.CorrespondentForNoticesContact.ForeName);
            Assert.Equal(surname, result.CorrespondentForNoticesContact.SurName);
        }

        [Fact]
        public async void SetAuthorisedRepresentative_WithNullRepresentative_ReturnsNull()
        {
            var builder = new GenerateFromXmlBuilder();

            var result = await builder.Build().SetAuthorisedRepresentative(null);

            Assert.Null(result);
        }

        [Fact]
        public async void SetAuthorisedRepresentative_WithNullRepresentativeOverseasProducer_ReturnsNull()
        {
            var builder = new GenerateFromXmlBuilder();

            var result = await builder.Build().SetAuthorisedRepresentative(new authorisedRepresentativeType());

            Assert.Null(result);
        }

        [Fact]
        public async void SetAuthorisedRepresentative_WithRepresentativeOverseasProducer_ReturnsOverseasProducerWithName()
        {
            var authorisedRepresentative = new authorisedRepresentativeType()
            {
                 overseasProducer = new overseasProducerType()
                 {
                      overseasProducerName = "Test overseas producer"
                 }
            };

            var builder = new GenerateFromXmlBuilder();
            var result = await builder.Build().SetAuthorisedRepresentative(authorisedRepresentative);

            Assert.Equal("Test overseas producer", authorisedRepresentative.overseasProducer.overseasProducerName);
            Assert.Null(authorisedRepresentative.overseasProducer.overseasContact);
        }

        [Fact]
        public async void SetAuthorisedRepresentative_WithRepresentativeOverseasProducer_WithContact_ReturnsOverseasProducerWithContact()
        {
            string forename = "forename";
            string surname = "surname";

            var authorisedRepresentative = new authorisedRepresentativeType()
            {
                overseasProducer = new overseasProducerType()
                {
                    overseasContact = new contactDetailsType()
                    {
                        address = new addressType()
                        {
                            country = countryType.UKENGLAND
                        },
                        forename = forename,
                        surname = surname
                    }
                }
            };

            var builder = new GenerateFromXmlBuilder();
            A.CallTo(() => builder.DataAccess.GetCountry(A<string>._)).Returns(new Country(Guid.NewGuid(), "BELARUS"));

            var result = await builder.Build().SetAuthorisedRepresentative(authorisedRepresentative);

            Assert.Equal(forename, authorisedRepresentative.overseasProducer.overseasContact.forename);
            Assert.Equal(surname, authorisedRepresentative.overseasProducer.overseasContact.surname);
        }

        private class GenerateFromXmlBuilder
        {
            public IXmlConverter XmlConverter;
            public IGenerateFromXmlDataAccess DataAccess;

            public GenerateFromXmlBuilder()
            {
                XmlConverter = A.Fake<IXmlConverter>();
                DataAccess = A.Fake<IGenerateFromXmlDataAccess>();
            }

            public GenerateFromXml Build()
            {
                return new GenerateFromXml(XmlConverter, DataAccess);
            }
        }
    }
}
