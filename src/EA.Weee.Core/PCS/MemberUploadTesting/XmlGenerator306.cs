using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EA.Weee.Core.PCS.MemberUploadTesting
{
    public class XmlGenerator306 : IXmlGenerator
    {
        private static readonly XNamespace ns = "http://www.environment-agency.gov.uk/WEEE/XMLSchema";

        public XDocument GenerateXml(ProducerList producerList)
        {
            XDocument xDoc = new XDocument();

            xDoc.Declaration = new XDeclaration("1.0", "utf-8", "yes");

            XElement xScheme = new XElement(ns + "scheme");
            xDoc.Add(xScheme);
            PopulateScheme(producerList, xScheme);

            return xDoc;
        }

        private void PopulateScheme(ProducerList producerList, XElement xScheme)
        {
            XElement xXSDVersion = new XElement(ns + "XSDVersion");
            xScheme.Add(xXSDVersion);
            switch (producerList.SchemaVersion)
            {
                case SchemaVersion.Version_3_04:
                    xXSDVersion.Value = "3.04";
                    break;

                case SchemaVersion.Version_3_06:
                    xXSDVersion.Value = "3.06";
                    break;

                default:
                    throw new NotSupportedException();
            }
            
            XElement xApprovalNo = new XElement(ns + "approvalNo");
            xScheme.Add(xApprovalNo);
            xApprovalNo.Value = producerList.ApprovalNumber ?? string.Empty;

            XElement xComplianceYear = new XElement(ns + "complianceYear");
            xScheme.Add(xComplianceYear);
            xComplianceYear.Value = producerList.ComplianceYear.ToString();

            XElement xTradingName = new XElement(ns + "tradingName");
            xScheme.Add(xTradingName);
            xTradingName.Value = producerList.TradingName ?? string.Empty;

            XElement xSchemeBusiness = new XElement(ns + "schemeBusiness");
            xScheme.Add(xSchemeBusiness);

            if (producerList.SchemeBusiness.Company != null)
            {
                XElement xCompany = new XElement(ns + "company");
                xSchemeBusiness.Add(xCompany);

                XElement xCompanyName = new XElement(ns + "companyName");
                xCompany.Add(xCompanyName);
                xCompanyName.Value = producerList.SchemeBusiness.Company.CompanyName ?? string.Empty;

                XElement xCompanyNumber = new XElement(ns + "companyNumber");
                xCompany.Add(xCompanyNumber);
                xCompanyNumber.Value = producerList.SchemeBusiness.Company.CompanyNumber ?? string.Empty;
            }

            if (producerList.SchemeBusiness.Partnership != null)
            {
                XElement xPartnership = new XElement(ns + "partnership");
                xSchemeBusiness.Add(xPartnership);

                XElement xPartnershipName = new XElement(ns + "partnershipName");
                xPartnership.Add(xPartnershipName);
                xPartnershipName.Value = producerList.SchemeBusiness.Partnership.PartnershipName ?? string.Empty;

                XElement xPartnershipList = new XElement(ns + "partnershipList");
                xPartnership.Add(xPartnershipList);

                foreach (string partner in producerList.SchemeBusiness.Partnership.PartnershipList)
                {
                    XElement xPartner = new XElement(ns + "partner");
                    xPartnershipList.Add(xPartner);
                    xPartner.Value = partner ?? string.Empty;
                }
            }

            XElement xProducerList = new XElement(ns + "producerList");
            xScheme.Add(xProducerList);
            PopulateProducerList(producerList, xProducerList);
        }

        private void PopulateProducerList(ProducerList producerList, XElement xProducerList)
        {
            foreach (Producer producer in producerList.Producers)
            {
                XElement xProducer = new XElement(ns + "producer");
                xProducerList.Add(xProducer);
                PopulateProducer(producer, xProducer);
            }
        }

        private void PopulateProducer(Producer producer, XElement xProducer)
        {
            XElement xStatus = new XElement(ns + "status");
            xProducer.Add(xStatus);
            
            switch (producer.Status)
            {
                case ProducerStatus.Insert:
                    xStatus.Value = "I";
                    break;

                case ProducerStatus.Amend:
                    xStatus.Value = "A";
                    break;

                default:
                    throw new NotSupportedException();
            }

            XElement xRegistrationNo = new XElement(ns + "registrationNo");
            xProducer.Add(xRegistrationNo);
            xRegistrationNo.Value = producer.RegistrationNumber ?? string.Empty;

            XElement xTradingName = new XElement(ns + "tradingName");
            xProducer.Add(xTradingName);
            xTradingName.Value = producer.TradingName ?? string.Empty;

            XElement xSICCodeList = new XElement(ns + "SICCodeList");
            xProducer.Add(xSICCodeList);

            foreach (string sicCode in producer.SICCodes)
            {
                XElement xSICCode = new XElement(ns + "SICCode");
                xSICCodeList.Add(xSICCode);
                xSICCode.Value = sicCode ?? string.Empty;
            }

            XElement xVATRegistered = new XElement(ns + "VATRegistered");
            xProducer.Add(xVATRegistered);
            xVATRegistered.Value = producer.VATRegistered ? "true" : "false";

            XElement xAnnualTurnover = new XElement(ns + "annualTurnover");
            xProducer.Add(xAnnualTurnover);
            xAnnualTurnover.Value = producer.AnnualTurnover.ToString();

            XElement xAnnualTurnoverBand = new XElement(ns + "annualTurnoverBand");
            xProducer.Add(xAnnualTurnoverBand);

            switch (producer.AnnualTurnoverBand)
            {
                case AnnualTurnoverBand.GreaterThanOneMillionPounds:
                    xAnnualTurnoverBand.Value = "Greater than one million pounds";
                    break;

                case AnnualTurnoverBand.LessThanOrEqualToOneMillionPounds:
                    xAnnualTurnoverBand.Value = "Less than or equal to one million pounds";
                    break;

                default:
                    throw new NotSupportedException();
            }

            XElement xEEEPlacedOnMarketBand = new XElement(ns + "eeePlacedOnMarketBand");
            xProducer.Add(xEEEPlacedOnMarketBand);

            switch (producer.EEEPlacedOnMarketBand)
            {
                case EEEPlacedOnMarketBand.LessThan5TEEEPlacedOnMarket:
                    xEEEPlacedOnMarketBand.Value = "Less than 5T EEE placed on market";
                    break;

                case EEEPlacedOnMarketBand.MoreThanOrEqualTo5TEEEPlacedOnMarket:
                    xEEEPlacedOnMarketBand.Value = "More than or equal to 5T EEE placed on market";
                    break;

                default:
                    throw new NotSupportedException();
            }

            XElement xObligationType = new XElement(ns + "obligationType");
            xProducer.Add(xObligationType);

            switch (producer.ObligationType)
            {
                case ObligationType.B2B:
                    xObligationType.Value = "B2B";
                    break;

                case ObligationType.B2C:
                    xObligationType.Value = "B2C";
                    break;

                case ObligationType.Both:
                    xObligationType.Value = "Both";
                    break;

                default:
                    throw new NotSupportedException();
            }

            XElement xProducerBrandNames = new XElement(ns + "producerBrandNames");
            xProducer.Add(xProducerBrandNames);

            foreach (string brandName in producer.BrandNames)
            {
                XElement xBrandName = new XElement(ns + "brandname");
                xProducerBrandNames.Add(xBrandName);
                xBrandName.Value = brandName ?? string.Empty;
            }

            XElement xProducerBusiness = new XElement(ns + "producerBusiness");
            xProducer.Add(xProducerBusiness);
            PopulateProducerBusiness(producer.ProducerBusiness, xProducerBusiness);

            XElement xAuthorizedRepresentative = new XElement(ns + "authorisedRepresentative");
            xProducer.Add(xAuthorizedRepresentative);
            PopulateAuthorizedRepresentative(producer.AuthorizedRepresentative, xAuthorizedRepresentative);

            if (producer.CeasedToExistDate != null)
            {
                XElement xCeasedToExistDate = new XElement(ns + "ceaseToExistDate");
                xProducer.Add(xCeasedToExistDate);
                xCeasedToExistDate.Value = producer.CeasedToExistDate.Value.ToString("yyyy-MM-dd");
            }

            XElement xSellingTechnique = new XElement(ns + "sellingTechnique");
            xProducer.Add(xSellingTechnique);

            switch (producer.SellingTechnique)
            {
                case SellingTechnique.DirectSellingToEndUser:
                    xSellingTechnique.Value = "Direct Selling to End User";
                    break;

                case SellingTechnique.IndirectSellingToEndUser:
                    xSellingTechnique.Value = "Indirect Selling to End User";
                    break;

                case SellingTechnique.Both:
                    xSellingTechnique.Value = "Both";
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void PopulateProducerBusiness(ProducerBusiness producerBusiness, XElement xProducerBusiness)
        {
            XElement xCorrespondentForNotices = new XElement(ns + "correspondentForNotices");
            xProducerBusiness.Add(xCorrespondentForNotices);

            if (producerBusiness.CorrespondentForNotices.ContactDetails != null)
            {
                XElement xContactDetails = new XElement(ns + "contactDetails");
                xCorrespondentForNotices.Add(xContactDetails);
                PopulateContactDetails(producerBusiness.CorrespondentForNotices.ContactDetails, xContactDetails);
            }

            if (producerBusiness.Partnership != null)
            {
                XElement xParnership = new XElement(ns + "partnership");
                xProducerBusiness.Add(xParnership);

                XElement xPartnershipName = new XElement(ns + "partnershipName");
                xParnership.Add(xPartnershipName);
                xPartnershipName.Value = producerBusiness.Partnership.PartnershipName ?? string.Empty;

                XElement xPartnershipList = new XElement(ns + "partnershipList");
                xParnership.Add(xPartnershipList);
                
                foreach (string partner in producerBusiness.Partnership.PartnershipList)
                {
                    XElement xPartner = new XElement(ns + "partner");
                    xPartnershipList.Add(xPartner);
                    xPartner.Value = partner ?? string.Empty;
                }

                XElement xPrincipalPlaceOfBusiness = new XElement(ns + "principalPlaceOfBusiness");
                xParnership.Add(xPrincipalPlaceOfBusiness);

                XElement xContactDetails = new XElement(ns + "contactDetails");
                xPrincipalPlaceOfBusiness.Add(xContactDetails);
                PopulateContactDetails(producerBusiness.Partnership.PrincipalPlaceOfBusiness, xContactDetails);
            }

            if (producerBusiness.Company != null)
            {
                XElement xCompany = new XElement(ns + "company");
                xProducerBusiness.Add(xCompany);

                XElement xCompanyName = new XElement(ns + "companyName");
                xCompany.Add(xCompanyName);
                xCompanyName.Value = producerBusiness.Company.CompanyName ?? string.Empty;

                XElement xCompanyNumber = new XElement(ns + "companyNumber");
                xCompany.Add(xCompanyNumber);
                xCompanyNumber.Value = producerBusiness.Company.CompanyNumber ?? string.Empty;

                XElement xRegisteredOffice = new XElement(ns + "registeredOffice");
                xCompany.Add(xRegisteredOffice);

                XElement xContactDetails = new XElement(ns + "contactDetails");
                xRegisteredOffice.Add(xContactDetails);
                PopulateContactDetails(producerBusiness.Company.RegisteredOffice, xContactDetails);
            }
        }

        private void PopulateAuthorizedRepresentative(AuthorizedRepresentative authorizedRepresentative, XElement xAuthorizedRepresentative)
        {
            if (authorizedRepresentative.OverseasProducer != null)
            {
                XElement xOverseasProducer = new XElement(ns + "overseasProducer");
                xAuthorizedRepresentative.Add(xOverseasProducer);

                XElement xOverseasProducerName = new XElement(ns + "overseasProducerName");
                xOverseasProducer.Add(xOverseasProducerName);
                xOverseasProducerName.Value = authorizedRepresentative.OverseasProducer.OverseasProducerName ?? string.Empty;

                if (authorizedRepresentative.OverseasProducer.ContactDetails != null)
                {
                    XElement xOverseasContact = new XElement(ns + "overseasContact");
                    xOverseasProducer.Add(xOverseasContact);
                    PopulateContactDetails(authorizedRepresentative.OverseasProducer.ContactDetails, xOverseasContact);
                }
            }
        }

        private void PopulateContactDetails(ContactDetails contactDetails, XElement xContactDetails)
        {
            XElement xTitle = new XElement(ns + "title");
            xContactDetails.Add(xTitle);
            xTitle.Value = contactDetails.Title ?? string.Empty;

            XElement xForename = new XElement(ns + "forename");
            xContactDetails.Add(xForename);
            xForename.Value = contactDetails.Forename ?? string.Empty;

            XElement xSurname = new XElement(ns + "surname");
            xContactDetails.Add(xSurname);
            xSurname.Value = contactDetails.Surname ?? string.Empty;

            XElement xPhoneLandLind = new XElement(ns + "phoneLandLine");
            xContactDetails.Add(xPhoneLandLind);
            xPhoneLandLind.Value = contactDetails.PhoneLandLine ?? string.Empty;

            XElement xPhoneMobile = new XElement(ns + "phoneMobile");
            xContactDetails.Add(xPhoneMobile);
            xPhoneMobile.Value = contactDetails.PhoneMobile ?? string.Empty;

            XElement xFax = new XElement(ns + "fax");
            xContactDetails.Add(xFax);
            xFax.Value = contactDetails.Fax ?? string.Empty;

            XElement xEmail = new XElement(ns + "email");
            xContactDetails.Add(xEmail);
            xEmail.Value = contactDetails.Email ?? string.Empty;

            XElement xAddress = new XElement(ns + "address");
            xContactDetails.Add(xAddress);
            PopulateAddress(contactDetails.Address, xAddress);
        }

        private void PopulateAddress(Address address, XElement xAddress)
        {
            XElement xPrimaryName = new XElement(ns + "primaryName");
            xAddress.Add(xPrimaryName);
            xPrimaryName.Value = address.PrimaryName ?? string.Empty;

            XElement xSecondaryName = new XElement(ns + "secondaryName");
            xAddress.Add(xSecondaryName);
            xSecondaryName.Value = address.SecondaryName ?? string.Empty;

            XElement xStreetName = new XElement(ns + "streetName");
            xAddress.Add(xStreetName);
            xStreetName.Value = address.StreetName ?? string.Empty;

            XElement xTown = new XElement(ns + "town");
            xAddress.Add(xTown);
            xTown.Value = address.Town ?? string.Empty;

            XElement xLocality = new XElement(ns + "locality");
            xAddress.Add(xLocality);
            xLocality.Value = address.Locality ?? string.Empty;

            XElement xAdministrativeArea = new XElement(ns + "administrativeArea");
            xAddress.Add(xAdministrativeArea);
            xAdministrativeArea.Value = address.AdministrativeArea ?? string.Empty;

            XElement xCountry = new XElement(ns + "country");
            xAddress.Add(xCountry);
            xCountry.Value = address.Country ?? string.Empty;

            XElement xPostCode = new XElement(ns + "postCode");
            xAddress.Add(xPostCode);
            xPostCode.Value = address.PostCode ?? string.Empty;
        }
    }
}
