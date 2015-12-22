namespace EA.Weee.RequestHandlers.Scheme.MemberUploadTesting
{
    using System;
    using System.Xml.Linq;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using Prsd.Core;
    using Xml;

    /// <summary>
    /// Creates an XML Document that can be used for testing the PCS member upload functionality
    /// based on the content of a ProducerList.
    /// 
    /// For now there is only one XmlGenerator which can generate XML for all versioXmlNamespace.MemberRegistration of the schema.
    /// If the structure or content of future versioXmlNamespace.MemberRegistration of the schema diverge substantially, then
    /// the stratergy pattern should be used to split the methods in this class into individual classes
    /// representing the different behaviours.
    /// </summary>
    public class XmlGenerator : IXmlGenerator
    {
        public XDocument GenerateXml(ProducerList producerList, ProducerListSettings settings)
        {
            Guard.ArgumentNotNull(() => producerList, producerList);
            Guard.ArgumentNotNull(() => settings, settings);

            XDocument xmlDoc = new XDocument();

            xmlDoc.Declaration = new XDeclaration("1.0", "utf-8", "yes");

            XElement xmlScheme = new XElement(XmlNamespace.MemberRegistration + "scheme");
            xmlDoc.Add(xmlScheme);

            // If we are creating an XML document that is deliberately invalid,
            // let's include a <foo/> element iXmlNamespace.MemberRegistrationide of the root element.
            if (settings.IncludeUnexpectedFooElement)
            {
                XElement xmlFoo = new XElement(XmlNamespace.MemberRegistration + "foo");
                xmlScheme.Add(xmlFoo);
            }

            PopulateScheme(producerList, xmlScheme);

            return xmlDoc;
        }

        private void PopulateScheme(ProducerList producerList, XElement xmlScheme)
        {
            XElement xmlXSDVersion = new XElement(XmlNamespace.MemberRegistration + "XSDVersion");
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
            
            XElement xmlApprovalNo = new XElement(XmlNamespace.MemberRegistration + "approvalNo");
            xmlScheme.Add(xmlApprovalNo);
            xmlApprovalNo.Value = producerList.ApprovalNumber ?? string.Empty;

            XElement xmlComplianceYear = new XElement(XmlNamespace.MemberRegistration + "complianceYear");
            xmlScheme.Add(xmlComplianceYear);
            xmlComplianceYear.Value = producerList.ComplianceYear.ToString();

            XElement xmlTradingName = new XElement(XmlNamespace.MemberRegistration + "tradingName");
            xmlScheme.Add(xmlTradingName);
            xmlTradingName.Value = producerList.TradingName ?? string.Empty;

            XElement xmlSchemeBusiness = new XElement(XmlNamespace.MemberRegistration + "schemeBusiness");
            xmlScheme.Add(xmlSchemeBusiness);

            if (producerList.SchemeBusiness.Company != null)
            {
                XElement xmlCompany = new XElement(XmlNamespace.MemberRegistration + "company");
                xmlSchemeBusiness.Add(xmlCompany);

                XElement xmlCompanyName = new XElement(XmlNamespace.MemberRegistration + "companyName");
                xmlCompany.Add(xmlCompanyName);
                xmlCompanyName.Value = producerList.SchemeBusiness.Company.CompanyName ?? string.Empty;

                XElement xmlCompanyNumber = new XElement(XmlNamespace.MemberRegistration + "companyNumber");
                xmlCompany.Add(xmlCompanyNumber);
                xmlCompanyNumber.Value = producerList.SchemeBusiness.Company.CompanyNumber ?? string.Empty;
            }

            if (producerList.SchemeBusiness.Partnership != null)
            {
                XElement xmlPartnership = new XElement(XmlNamespace.MemberRegistration + "partnership");
                xmlSchemeBusiness.Add(xmlPartnership);

                XElement xmlPartnershipName = new XElement(XmlNamespace.MemberRegistration + "partnershipName");
                xmlPartnership.Add(xmlPartnershipName);
                xmlPartnershipName.Value = producerList.SchemeBusiness.Partnership.PartnershipName ?? string.Empty;

                XElement xmlPartnershipList = new XElement(XmlNamespace.MemberRegistration + "partnershipList");
                xmlPartnership.Add(xmlPartnershipList);

                foreach (string partner in producerList.SchemeBusiness.Partnership.PartnershipList)
                {
                    XElement xmlPartner = new XElement(XmlNamespace.MemberRegistration + "partner");
                    xmlPartnershipList.Add(xmlPartner);
                    xmlPartner.Value = partner ?? string.Empty;
                }
            }

            XElement xmlProducerList = new XElement(XmlNamespace.MemberRegistration + "producerList");
            xmlScheme.Add(xmlProducerList);
            PopulateProducerList(producerList, xmlProducerList);
        }

        private void PopulateProducerList(ProducerList producerList, XElement xmlProducerList)
        {
            foreach (Producer producer in producerList.Producers)
            {
                XElement xmlProducer = new XElement(XmlNamespace.MemberRegistration + "producer");
                xmlProducerList.Add(xmlProducer);
                PopulateProducer(producer, xmlProducer);
            }
        }

        private void PopulateProducer(Producer producer, XElement xmlProducer)
        {
            XElement xmlStatus = new XElement(XmlNamespace.MemberRegistration + "status");
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

            XElement xmlRegistrationNo = new XElement(XmlNamespace.MemberRegistration + "registrationNo");
            xmlProducer.Add(xmlRegistrationNo);
            xmlRegistrationNo.Value = producer.RegistrationNumber ?? string.Empty;

            XElement xmlTradingName = new XElement(XmlNamespace.MemberRegistration + "tradingName");
            xmlProducer.Add(xmlTradingName);
            xmlTradingName.Value = producer.TradingName ?? string.Empty;

            XElement xmlSICCodeList = new XElement(XmlNamespace.MemberRegistration + "SICCodeList");
            xmlProducer.Add(xmlSICCodeList);

            foreach (string sicCode in producer.SICCodes)
            {
                XElement xmlSICCode = new XElement(XmlNamespace.MemberRegistration + "SICCode");
                xmlSICCodeList.Add(xmlSICCode);
                xmlSICCode.Value = sicCode ?? string.Empty;
            }

            XElement xmlVATRegistered = new XElement(XmlNamespace.MemberRegistration + "VATRegistered");
            xmlProducer.Add(xmlVATRegistered);
            xmlVATRegistered.Value = producer.VATRegistered ? "true" : "false";

            XElement xmlAnnualTurnover = new XElement(XmlNamespace.MemberRegistration + "annualTurnover");
            xmlProducer.Add(xmlAnnualTurnover);
            xmlAnnualTurnover.Value = producer.AnnualTurnover.ToString();

            XElement xmlAnnualTurnoverBand = new XElement(XmlNamespace.MemberRegistration + "annualTurnoverBand");
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

            XElement xmlEEEPlacedOnMarketBand = new XElement(XmlNamespace.MemberRegistration + "eeePlacedOnMarketBand");
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

            XElement xmlObligationType = new XElement(XmlNamespace.MemberRegistration + "obligationType");
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

            XElement xmlProducerBrandNames = new XElement(XmlNamespace.MemberRegistration + "producerBrandNames");
            xmlProducer.Add(xmlProducerBrandNames);

            foreach (string brandName in producer.BrandNames)
            {
                XElement xmlBrandName = new XElement(XmlNamespace.MemberRegistration + "brandname");
                xmlProducerBrandNames.Add(xmlBrandName);
                xmlBrandName.Value = brandName ?? string.Empty;
            }

            XElement xmlProducerBusiness = new XElement(XmlNamespace.MemberRegistration + "producerBusiness");
            xmlProducer.Add(xmlProducerBusiness);
            PopulateProducerBusiness(producer.ProducerBusiness, xmlProducerBusiness);

            if (producer.AuthorizedRepresentative != null)
            {
                XElement xmlAuthorizedRepresentative = new XElement(XmlNamespace.MemberRegistration + "authorisedRepresentative");
                xmlProducer.Add(xmlAuthorizedRepresentative);
                PopulateAuthorizedRepresentative(producer.AuthorizedRepresentative, xmlAuthorizedRepresentative);
            }

            if (producer.CeasedToExistDate != null)
            {
                XElement xmlCeasedToExistDate = new XElement(XmlNamespace.MemberRegistration + "ceaseToExistDate");
                xmlProducer.Add(xmlCeasedToExistDate);
                xmlCeasedToExistDate.Value = producer.CeasedToExistDate.Value.ToString("yyyy-MM-dd");
            }

            XElement xmlSellingTechnique = new XElement(XmlNamespace.MemberRegistration + "sellingTechnique");
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
            XElement xmlCorrespondentForNotices = new XElement(XmlNamespace.MemberRegistration + "correspondentForNotices");
            xmlProducerBusiness.Add(xmlCorrespondentForNotices);

            if (producerBusiness.CorrespondentForNotices.ContactDetails != null)
            {
                XElement xmlContactDetails = new XElement(XmlNamespace.MemberRegistration + "contactDetails");
                xmlCorrespondentForNotices.Add(xmlContactDetails);
                PopulateContactDetails(producerBusiness.CorrespondentForNotices.ContactDetails, xmlContactDetails);
            }

            if (producerBusiness.Partnership != null)
            {
                XElement xmlParnership = new XElement(XmlNamespace.MemberRegistration + "partnership");
                xmlProducerBusiness.Add(xmlParnership);

                XElement xmlPartnershipName = new XElement(XmlNamespace.MemberRegistration + "partnershipName");
                xmlParnership.Add(xmlPartnershipName);
                xmlPartnershipName.Value = producerBusiness.Partnership.PartnershipName ?? string.Empty;

                XElement xmlPartnershipList = new XElement(XmlNamespace.MemberRegistration + "partnershipList");
                xmlParnership.Add(xmlPartnershipList);
                
                foreach (string partner in producerBusiness.Partnership.PartnershipList)
                {
                    XElement xmlPartner = new XElement(XmlNamespace.MemberRegistration + "partner");
                    xmlPartnershipList.Add(xmlPartner);
                    xmlPartner.Value = partner ?? string.Empty;
                }

                XElement xmlPrincipalPlaceOfBusiness = new XElement(XmlNamespace.MemberRegistration + "principalPlaceOfBusiness");
                xmlParnership.Add(xmlPrincipalPlaceOfBusiness);

                XElement xmlContactDetails = new XElement(XmlNamespace.MemberRegistration + "contactDetails");
                xmlPrincipalPlaceOfBusiness.Add(xmlContactDetails);
                PopulateContactDetails(producerBusiness.Partnership.PrincipalPlaceOfBusiness, xmlContactDetails);
            }

            if (producerBusiness.Company != null)
            {
                XElement xmlCompany = new XElement(XmlNamespace.MemberRegistration + "company");
                xmlProducerBusiness.Add(xmlCompany);

                XElement xmlCompanyName = new XElement(XmlNamespace.MemberRegistration + "companyName");
                xmlCompany.Add(xmlCompanyName);
                xmlCompanyName.Value = producerBusiness.Company.CompanyName ?? string.Empty;

                XElement xmlCompanyNumber = new XElement(XmlNamespace.MemberRegistration + "companyNumber");
                xmlCompany.Add(xmlCompanyNumber);
                xmlCompanyNumber.Value = producerBusiness.Company.CompanyNumber ?? string.Empty;

                XElement xmlRegisteredOffice = new XElement(XmlNamespace.MemberRegistration + "registeredOffice");
                xmlCompany.Add(xmlRegisteredOffice);

                XElement xmlContactDetails = new XElement(XmlNamespace.MemberRegistration + "contactDetails");
                xmlRegisteredOffice.Add(xmlContactDetails);
                PopulateContactDetails(producerBusiness.Company.RegisteredOffice, xmlContactDetails);
            }
        }

        private void PopulateAuthorizedRepresentative(AuthorizedRepresentative authorizedRepresentative, XElement xmlAuthorizedRepresentative)
        {
            if (authorizedRepresentative.OverseasProducer != null)
            {
                XElement xmlOverseasProducer = new XElement(XmlNamespace.MemberRegistration + "overseasProducer");
                xmlAuthorizedRepresentative.Add(xmlOverseasProducer);

                XElement xmlOverseasProducerName = new XElement(XmlNamespace.MemberRegistration + "overseasProducerName");
                xmlOverseasProducer.Add(xmlOverseasProducerName);
                xmlOverseasProducerName.Value = authorizedRepresentative.OverseasProducer.OverseasProducerName ?? string.Empty;

                if (authorizedRepresentative.OverseasProducer.ContactDetails != null)
                {
                    XElement xmlOverseasContact = new XElement(XmlNamespace.MemberRegistration + "overseasContact");
                    xmlOverseasProducer.Add(xmlOverseasContact);
                    PopulateContactDetails(authorizedRepresentative.OverseasProducer.ContactDetails, xmlOverseasContact);
                }
            }
        }

        private void PopulateContactDetails(ContactDetails contactDetails, XElement xmlContactDetails)
        {
            XElement xmlTitle = new XElement(XmlNamespace.MemberRegistration + "title");
            xmlContactDetails.Add(xmlTitle);
            xmlTitle.Value = contactDetails.Title ?? string.Empty;

            XElement xmlForename = new XElement(XmlNamespace.MemberRegistration + "forename");
            xmlContactDetails.Add(xmlForename);
            xmlForename.Value = contactDetails.Forename ?? string.Empty;

            XElement xmlSurname = new XElement(XmlNamespace.MemberRegistration + "surname");
            xmlContactDetails.Add(xmlSurname);
            xmlSurname.Value = contactDetails.Surname ?? string.Empty;

            XElement xmlPhoneLandLind = new XElement(XmlNamespace.MemberRegistration + "phoneLandLine");
            xmlContactDetails.Add(xmlPhoneLandLind);
            xmlPhoneLandLind.Value = contactDetails.PhoneLandLine ?? string.Empty;

            XElement xmlPhoneMobile = new XElement(XmlNamespace.MemberRegistration + "phoneMobile");
            xmlContactDetails.Add(xmlPhoneMobile);
            xmlPhoneMobile.Value = contactDetails.PhoneMobile ?? string.Empty;

            XElement xmlFax = new XElement(XmlNamespace.MemberRegistration + "fax");
            xmlContactDetails.Add(xmlFax);
            xmlFax.Value = contactDetails.Fax ?? string.Empty;

            XElement xmlEmail = new XElement(XmlNamespace.MemberRegistration + "email");
            xmlContactDetails.Add(xmlEmail);
            xmlEmail.Value = contactDetails.Email ?? string.Empty;

            XElement xmlAddress = new XElement(XmlNamespace.MemberRegistration + "address");
            xmlContactDetails.Add(xmlAddress);
            PopulateAddress(contactDetails.Address, xmlAddress);
        }

        private void PopulateAddress(Address address, XElement xmlAddress)
        {
            XElement xmlPrimaryName = new XElement(XmlNamespace.MemberRegistration + "primaryName");
            xmlAddress.Add(xmlPrimaryName);
            xmlPrimaryName.Value = address.PrimaryName ?? string.Empty;

            XElement xmlSecondaryName = new XElement(XmlNamespace.MemberRegistration + "secondaryName");
            xmlAddress.Add(xmlSecondaryName);
            xmlSecondaryName.Value = address.SecondaryName ?? string.Empty;

            XElement xmlStreetName = new XElement(XmlNamespace.MemberRegistration + "streetName");
            xmlAddress.Add(xmlStreetName);
            xmlStreetName.Value = address.StreetName ?? string.Empty;

            XElement xmlTown = new XElement(XmlNamespace.MemberRegistration + "town");
            xmlAddress.Add(xmlTown);
            xmlTown.Value = address.Town ?? string.Empty;

            XElement xmlLocality = new XElement(XmlNamespace.MemberRegistration + "locality");
            xmlAddress.Add(xmlLocality);
            xmlLocality.Value = address.Locality ?? string.Empty;

            XElement xmlAdministrativeArea = new XElement(XmlNamespace.MemberRegistration + "administrativeArea");
            xmlAddress.Add(xmlAdministrativeArea);
            xmlAdministrativeArea.Value = address.AdministrativeArea ?? string.Empty;

            XElement xmlCountry = new XElement(XmlNamespace.MemberRegistration + "country");
            xmlAddress.Add(xmlCountry);
            xmlCountry.Value = address.Country ?? string.Empty;

            if (address.IsUkBased)
            {
                XElement xmlPostCode = new XElement(XmlNamespace.MemberRegistration + "postCode");
                xmlAddress.Add(xmlPostCode);
                xmlPostCode.Value = address.PostCode ?? string.Empty;
            }
            else
            {
                XElement xmlInternationalPostCode = new XElement(XmlNamespace.MemberRegistration + "internationalPostCode");
                xmlAddress.Add(xmlInternationalPostCode);
                xmlInternationalPostCode.Value = address.PostCode ?? string.Empty;
            }
        }
    }
}
