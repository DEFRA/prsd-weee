namespace EA.Weee.RequestHandlers.Scheme.MemberUploadTesting
{
    using System;
    using System.Xml.Linq;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using Prsd.Core;

    /// <summary>
    /// Creates an XML Document that can be used for testing the PCS member upload functionality
    /// based on the content of a ProducerList.
    /// 
    /// For now there is only one XmlGenerator which can generate XML for all versions of the schema.
    /// If the structure or content of future versions of the schema diverge substantially, then
    /// the stratergy pattern should be used to split the methods in this class into individual classes
    /// representing the different behaviours.
    /// </summary>
    public class XmlGenerator : IXmlGenerator
    {
        private static readonly XNamespace ns = "http://www.environment-agency.gov.uk/WEEE/XMLSchema";

        public XDocument GenerateXml(ProducerList producerList, ProducerListSettings settings)
        {
            Guard.ArgumentNotNull(() => producerList, producerList);
            Guard.ArgumentNotNull(() => settings, settings);

            XDocument xmlDoc = new XDocument();

            xmlDoc.Declaration = new XDeclaration("1.0", "utf-8", "yes");

            XElement xmlScheme = new XElement(ns + "scheme");
            xmlDoc.Add(xmlScheme);

            // If we are creating an XML document that is deliberately invalid,
            // let's include a <foo/> element inside of the root element.
            if (settings.IncludeUnexpectedFooElement)
            {
                XElement xmlFoo = new XElement(ns + "foo");
                xmlScheme.Add(xmlFoo);
            }

            PopulateScheme(producerList, xmlScheme);

            return xmlDoc;
        }

        private void PopulateScheme(ProducerList producerList, XElement xmlScheme)
        {
            XElement xmlXSDVersion = new XElement(ns + "XSDVersion");
            xmlScheme.Add(xmlXSDVersion);
            switch (producerList.SchemaVersion)
            {
                case MemberRegistrationSchemaVersion.Version_3_04:
                    xmlXSDVersion.Value = "3.04";
                    break;

                case MemberRegistrationSchemaVersion.Version_3_06:
                    xmlXSDVersion.Value = "3.06";
                    break;

                case MemberRegistrationSchemaVersion.Version_3_07:
                    xmlXSDVersion.Value = "3.07";
                    break;

                default:
                    throw new NotSupportedException();
            }
            
            XElement xmlApprovalNo = new XElement(ns + "approvalNo");
            xmlScheme.Add(xmlApprovalNo);
            xmlApprovalNo.Value = producerList.ApprovalNumber ?? string.Empty;

            XElement xmlComplianceYear = new XElement(ns + "complianceYear");
            xmlScheme.Add(xmlComplianceYear);
            xmlComplianceYear.Value = producerList.ComplianceYear.ToString();

            XElement xmlTradingName = new XElement(ns + "tradingName");
            xmlScheme.Add(xmlTradingName);
            xmlTradingName.Value = producerList.TradingName ?? string.Empty;

            XElement xmlSchemeBusiness = new XElement(ns + "schemeBusiness");
            xmlScheme.Add(xmlSchemeBusiness);

            if (producerList.SchemeBusiness.Company != null)
            {
                XElement xmlCompany = new XElement(ns + "company");
                xmlSchemeBusiness.Add(xmlCompany);

                XElement xmlCompanyName = new XElement(ns + "companyName");
                xmlCompany.Add(xmlCompanyName);
                xmlCompanyName.Value = producerList.SchemeBusiness.Company.CompanyName ?? string.Empty;

                XElement xmlCompanyNumber = new XElement(ns + "companyNumber");
                xmlCompany.Add(xmlCompanyNumber);
                xmlCompanyNumber.Value = producerList.SchemeBusiness.Company.CompanyNumber ?? string.Empty;
            }

            if (producerList.SchemeBusiness.Partnership != null)
            {
                XElement xmlPartnership = new XElement(ns + "partnership");
                xmlSchemeBusiness.Add(xmlPartnership);

                XElement xmlPartnershipName = new XElement(ns + "partnershipName");
                xmlPartnership.Add(xmlPartnershipName);
                xmlPartnershipName.Value = producerList.SchemeBusiness.Partnership.PartnershipName ?? string.Empty;

                XElement xmlPartnershipList = new XElement(ns + "partnershipList");
                xmlPartnership.Add(xmlPartnershipList);

                foreach (string partner in producerList.SchemeBusiness.Partnership.PartnershipList)
                {
                    XElement xmlPartner = new XElement(ns + "partner");
                    xmlPartnershipList.Add(xmlPartner);
                    xmlPartner.Value = partner ?? string.Empty;
                }
            }

            XElement xmlProducerList = new XElement(ns + "producerList");
            xmlScheme.Add(xmlProducerList);
            PopulateProducerList(producerList, xmlProducerList);
        }

        private void PopulateProducerList(ProducerList producerList, XElement xmlProducerList)
        {
            foreach (Producer producer in producerList.Producers)
            {
                XElement xmlProducer = new XElement(ns + "producer");
                xmlProducerList.Add(xmlProducer);
                PopulateProducer(producer, xmlProducer);
            }
        }

        private void PopulateProducer(Producer producer, XElement xmlProducer)
        {
            XElement xmlStatus = new XElement(ns + "status");
            xmlProducer.Add(xmlStatus);
            
            switch (producer.Status)
            {
                case ProducerStatus.Insert:
                    xmlStatus.Value = "I";
                    break;

                case ProducerStatus.Amend:
                    xmlStatus.Value = "A";
                    break;

                default:
                    throw new NotSupportedException();
            }

            XElement xmlRegistrationNo = new XElement(ns + "registrationNo");
            xmlProducer.Add(xmlRegistrationNo);
            xmlRegistrationNo.Value = producer.RegistrationNumber ?? string.Empty;

            XElement xmlTradingName = new XElement(ns + "tradingName");
            xmlProducer.Add(xmlTradingName);
            xmlTradingName.Value = producer.TradingName ?? string.Empty;

            XElement xmlSICCodeList = new XElement(ns + "SICCodeList");
            xmlProducer.Add(xmlSICCodeList);

            foreach (string sicCode in producer.SICCodes)
            {
                XElement xmlSICCode = new XElement(ns + "SICCode");
                xmlSICCodeList.Add(xmlSICCode);
                xmlSICCode.Value = sicCode ?? string.Empty;
            }

            XElement xmlVATRegistered = new XElement(ns + "VATRegistered");
            xmlProducer.Add(xmlVATRegistered);
            xmlVATRegistered.Value = producer.VATRegistered ? "true" : "false";

            XElement xmlAnnualTurnover = new XElement(ns + "annualTurnover");
            xmlProducer.Add(xmlAnnualTurnover);
            xmlAnnualTurnover.Value = producer.AnnualTurnover.ToString();

            XElement xmlAnnualTurnoverBand = new XElement(ns + "annualTurnoverBand");
            xmlProducer.Add(xmlAnnualTurnoverBand);

            switch (producer.AnnualTurnoverBand)
            {
                case AnnualTurnoverBand.GreaterThanOneMillionPounds:
                    xmlAnnualTurnoverBand.Value = "Greater than one million pounds";
                    break;

                case AnnualTurnoverBand.LessThanOrEqualToOneMillionPounds:
                    xmlAnnualTurnoverBand.Value = "Less than or equal to one million pounds";
                    break;

                default:
                    throw new NotSupportedException();
            }

            XElement xmlEEEPlacedOnMarketBand = new XElement(ns + "eeePlacedOnMarketBand");
            xmlProducer.Add(xmlEEEPlacedOnMarketBand);

            switch (producer.EEEPlacedOnMarketBand)
            {
                case EEEPlacedOnMarketBand.LessThan5TEEEPlacedOnMarket:
                    xmlEEEPlacedOnMarketBand.Value = "Less than 5T EEE placed on market";
                    break;

                case EEEPlacedOnMarketBand.MoreThanOrEqualTo5TEEEPlacedOnMarket:
                    xmlEEEPlacedOnMarketBand.Value = "More than or equal to 5T EEE placed on market";
                    break;

                default:
                    throw new NotSupportedException();
            }

            XElement xmlObligationType = new XElement(ns + "obligationType");
            xmlProducer.Add(xmlObligationType);

            switch (producer.ObligationType)
            {
                case ObligationType.B2B:
                    xmlObligationType.Value = "B2B";
                    break;

                case ObligationType.B2C:
                    xmlObligationType.Value = "B2C";
                    break;

                case ObligationType.Both:
                    xmlObligationType.Value = "Both";
                    break;

                default:
                    throw new NotSupportedException();
            }

            XElement xmlProducerBrandNames = new XElement(ns + "producerBrandNames");
            xmlProducer.Add(xmlProducerBrandNames);

            foreach (string brandName in producer.BrandNames)
            {
                XElement xmlBrandName = new XElement(ns + "brandname");
                xmlProducerBrandNames.Add(xmlBrandName);
                xmlBrandName.Value = brandName ?? string.Empty;
            }

            XElement xmlProducerBusiness = new XElement(ns + "producerBusiness");
            xmlProducer.Add(xmlProducerBusiness);
            PopulateProducerBusiness(producer.ProducerBusiness, xmlProducerBusiness);

            if (producer.AuthorizedRepresentative != null)
            {
                XElement xmlAuthorizedRepresentative = new XElement(ns + "authorisedRepresentative");
                xmlProducer.Add(xmlAuthorizedRepresentative);
                PopulateAuthorizedRepresentative(producer.AuthorizedRepresentative, xmlAuthorizedRepresentative);
            }

            if (producer.CeasedToExistDate != null)
            {
                XElement xmlCeasedToExistDate = new XElement(ns + "ceaseToExistDate");
                xmlProducer.Add(xmlCeasedToExistDate);
                xmlCeasedToExistDate.Value = producer.CeasedToExistDate.Value.ToString("yyyy-MM-dd");
            }

            XElement xmlSellingTechnique = new XElement(ns + "sellingTechnique");
            xmlProducer.Add(xmlSellingTechnique);

            switch (producer.SellingTechnique)
            {
                case SellingTechnique.DirectSellingToEndUser:
                    xmlSellingTechnique.Value = "Direct Selling to End User";
                    break;

                case SellingTechnique.IndirectSellingToEndUser:
                    xmlSellingTechnique.Value = "Indirect Selling to End User";
                    break;

                case SellingTechnique.Both:
                    xmlSellingTechnique.Value = "Both";
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void PopulateProducerBusiness(ProducerBusiness producerBusiness, XElement xmlProducerBusiness)
        {
            XElement xmlCorrespondentForNotices = new XElement(ns + "correspondentForNotices");
            xmlProducerBusiness.Add(xmlCorrespondentForNotices);

            if (producerBusiness.CorrespondentForNotices.ContactDetails != null)
            {
                XElement xmlContactDetails = new XElement(ns + "contactDetails");
                xmlCorrespondentForNotices.Add(xmlContactDetails);
                PopulateContactDetails(producerBusiness.CorrespondentForNotices.ContactDetails, xmlContactDetails);
            }

            if (producerBusiness.Partnership != null)
            {
                XElement xmlParnership = new XElement(ns + "partnership");
                xmlProducerBusiness.Add(xmlParnership);

                XElement xmlPartnershipName = new XElement(ns + "partnershipName");
                xmlParnership.Add(xmlPartnershipName);
                xmlPartnershipName.Value = producerBusiness.Partnership.PartnershipName ?? string.Empty;

                XElement xmlPartnershipList = new XElement(ns + "partnershipList");
                xmlParnership.Add(xmlPartnershipList);
                
                foreach (string partner in producerBusiness.Partnership.PartnershipList)
                {
                    XElement xmlPartner = new XElement(ns + "partner");
                    xmlPartnershipList.Add(xmlPartner);
                    xmlPartner.Value = partner ?? string.Empty;
                }

                XElement xmlPrincipalPlaceOfBusiness = new XElement(ns + "principalPlaceOfBusiness");
                xmlParnership.Add(xmlPrincipalPlaceOfBusiness);

                XElement xmlContactDetails = new XElement(ns + "contactDetails");
                xmlPrincipalPlaceOfBusiness.Add(xmlContactDetails);
                PopulateContactDetails(producerBusiness.Partnership.PrincipalPlaceOfBusiness, xmlContactDetails);
            }

            if (producerBusiness.Company != null)
            {
                XElement xmlCompany = new XElement(ns + "company");
                xmlProducerBusiness.Add(xmlCompany);

                XElement xmlCompanyName = new XElement(ns + "companyName");
                xmlCompany.Add(xmlCompanyName);
                xmlCompanyName.Value = producerBusiness.Company.CompanyName ?? string.Empty;

                XElement xmlCompanyNumber = new XElement(ns + "companyNumber");
                xmlCompany.Add(xmlCompanyNumber);
                xmlCompanyNumber.Value = producerBusiness.Company.CompanyNumber ?? string.Empty;

                XElement xmlRegisteredOffice = new XElement(ns + "registeredOffice");
                xmlCompany.Add(xmlRegisteredOffice);

                XElement xmlContactDetails = new XElement(ns + "contactDetails");
                xmlRegisteredOffice.Add(xmlContactDetails);
                PopulateContactDetails(producerBusiness.Company.RegisteredOffice, xmlContactDetails);
            }
        }

        private void PopulateAuthorizedRepresentative(AuthorizedRepresentative authorizedRepresentative, XElement xmlAuthorizedRepresentative)
        {
            if (authorizedRepresentative.OverseasProducer != null)
            {
                XElement xmlOverseasProducer = new XElement(ns + "overseasProducer");
                xmlAuthorizedRepresentative.Add(xmlOverseasProducer);

                XElement xmlOverseasProducerName = new XElement(ns + "overseasProducerName");
                xmlOverseasProducer.Add(xmlOverseasProducerName);
                xmlOverseasProducerName.Value = authorizedRepresentative.OverseasProducer.OverseasProducerName ?? string.Empty;

                if (authorizedRepresentative.OverseasProducer.ContactDetails != null)
                {
                    XElement xmlOverseasContact = new XElement(ns + "overseasContact");
                    xmlOverseasProducer.Add(xmlOverseasContact);
                    PopulateContactDetails(authorizedRepresentative.OverseasProducer.ContactDetails, xmlOverseasContact);
                }
            }
        }

        private void PopulateContactDetails(ContactDetails contactDetails, XElement xmlContactDetails)
        {
            XElement xmlTitle = new XElement(ns + "title");
            xmlContactDetails.Add(xmlTitle);
            xmlTitle.Value = contactDetails.Title ?? string.Empty;

            XElement xmlForename = new XElement(ns + "forename");
            xmlContactDetails.Add(xmlForename);
            xmlForename.Value = contactDetails.Forename ?? string.Empty;

            XElement xmlSurname = new XElement(ns + "surname");
            xmlContactDetails.Add(xmlSurname);
            xmlSurname.Value = contactDetails.Surname ?? string.Empty;

            XElement xmlPhoneLandLind = new XElement(ns + "phoneLandLine");
            xmlContactDetails.Add(xmlPhoneLandLind);
            xmlPhoneLandLind.Value = contactDetails.PhoneLandLine ?? string.Empty;

            XElement xmlPhoneMobile = new XElement(ns + "phoneMobile");
            xmlContactDetails.Add(xmlPhoneMobile);
            xmlPhoneMobile.Value = contactDetails.PhoneMobile ?? string.Empty;

            XElement xmlFax = new XElement(ns + "fax");
            xmlContactDetails.Add(xmlFax);
            xmlFax.Value = contactDetails.Fax ?? string.Empty;

            XElement xmlEmail = new XElement(ns + "email");
            xmlContactDetails.Add(xmlEmail);
            xmlEmail.Value = contactDetails.Email ?? string.Empty;

            XElement xmlAddress = new XElement(ns + "address");
            xmlContactDetails.Add(xmlAddress);
            PopulateAddress(contactDetails.Address, xmlAddress);
        }

        private void PopulateAddress(Address address, XElement xmlAddress)
        {
            XElement xmlPrimaryName = new XElement(ns + "primaryName");
            xmlAddress.Add(xmlPrimaryName);
            xmlPrimaryName.Value = address.PrimaryName ?? string.Empty;

            XElement xmlSecondaryName = new XElement(ns + "secondaryName");
            xmlAddress.Add(xmlSecondaryName);
            xmlSecondaryName.Value = address.SecondaryName ?? string.Empty;

            XElement xmlStreetName = new XElement(ns + "streetName");
            xmlAddress.Add(xmlStreetName);
            xmlStreetName.Value = address.StreetName ?? string.Empty;

            XElement xmlTown = new XElement(ns + "town");
            xmlAddress.Add(xmlTown);
            xmlTown.Value = address.Town ?? string.Empty;

            XElement xmlLocality = new XElement(ns + "locality");
            xmlAddress.Add(xmlLocality);
            xmlLocality.Value = address.Locality ?? string.Empty;

            XElement xmlAdministrativeArea = new XElement(ns + "administrativeArea");
            xmlAddress.Add(xmlAdministrativeArea);
            xmlAdministrativeArea.Value = address.AdministrativeArea ?? string.Empty;

            XElement xmlCountry = new XElement(ns + "country");
            xmlAddress.Add(xmlCountry);
            xmlCountry.Value = address.Country ?? string.Empty;

            if (address.IsUkBased)
            {
                XElement xmlPostCode = new XElement(ns + "postCode");
                xmlAddress.Add(xmlPostCode);
                xmlPostCode.Value = address.PostCode ?? string.Empty;
            }
            else
            {
                XElement xmlInternationalPostCode = new XElement(ns + "internationalPostCode");
                xmlAddress.Add(xmlInternationalPostCode);
                xmlInternationalPostCode.Value = address.PostCode ?? string.Empty;
            }
        }
    }
}
