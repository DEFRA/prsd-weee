namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using DataAccess;
    using Domain;
    using Domain.PCS;
    using Domain.Producer;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public static class BuildProducerDataFromXml
    {
        public static async Task<List<Producer>> SetProducerData(Guid schemeId, MemberUpload memberUpload, WeeeContext context, string xmlData)
        {
            List<Producer> producers = new List<Producer>();

            var doc = XDocument.Parse(xmlData, LoadOptions.SetLineInfo);
            var deserialzedXml = new XmlSerializer(typeof(schemeType)).Deserialize(doc.CreateReader());
            schemeType scheme = (schemeType)deserialzedXml;
            foreach (producerType producerData in scheme.producerList)
            {
                List<BrandName> brandNames = new List<BrandName>();
                foreach (string name in producerData.producerBrandNames)
                {
                    BrandName brand = new BrandName(name);
                    brandNames.Add(brand);
                }

                List<SICCode> codes = new List<SICCode>();
                foreach (string code in producerData.SICCodeList)
                {
                    SICCode siscode = new SICCode(code);
                    codes.Add(siscode);
                }

                ProducerBusiness producerBusiness = await SetProducerBusiness(producerData.producerBusiness, context);

                AuthorisedRepresentative authorisedRepresentative = await SetAuthorisedRepresentative(producerData.authorisedRepresentative, context);

                int value = (int)producerData.eeePlacedOnMarketBand;
                EEEPlacedOnMarketBandType eeebandType = Enumeration.FromValue<EEEPlacedOnMarketBandType>(value);
          
                value = (int)producerData.sellingTechnique;
                SellingTechniqueType sellingTechniqueType = Enumeration.FromValue<SellingTechniqueType>(value);
        
                value = (int)producerData.obligationType;
                ObligationType obligationType = Enumeration.FromValue<ObligationType>(value);

                value = (int)producerData.annualTurnoverBand;
                AnnualTurnOverBandType annualturnoverType = Enumeration.FromValue<AnnualTurnOverBandType>(value);

                DateTime? ceaseDate = null;
                if (producerData.ceaseToExistDateSpecified)
                {
                    ceaseDate = producerData.ceaseToExistDate;
                }

                Producer producer = new Producer(schemeId, memberUpload, producerBusiness,
                    authorisedRepresentative,
                    SystemTime.UtcNow, (decimal)producerData.annualTurnover, producerData.VATRegistered,
                    producerData.registrationNo, ceaseDate,
                    producerData.tradingName,
                    eeebandType,
                    sellingTechniqueType,
                    obligationType,
                    annualturnoverType,
                    brandNames, codes);

                producers.Add(producer);
            }

            return producers;
        }

        private static async Task<AuthorisedRepresentative> SetAuthorisedRepresentative(authorisedRepresentativeType representative, WeeeContext context)
        {
            if (representative.overseasProducer == null)
            {
                return null;
            }

            List<ProducerContact> contacts = new List<ProducerContact>();
            if (representative.overseasProducer.overseasContact != null)
            {
                foreach (contactDetailsType detail in representative.overseasProducer.overseasContact)
                {
                    ProducerContact producerContact = await GetProducerContact(detail, context);
                    contacts.Add(producerContact);
                }
            }
            AuthorisedRepresentative overSeasAuthorisedRepresentative = new AuthorisedRepresentative(representative.overseasProducer.overseasProducerName, contacts.FirstOrDefault());
            return overSeasAuthorisedRepresentative;
        }

        private static async Task<ProducerBusiness> SetProducerBusiness(producerBusinessType producerBusiness, WeeeContext context)
        {
            object item = producerBusiness.Item;
            ProducerContact correspondentForNoticeContact = null;
            if (producerBusiness.correspondentForNotices.contactDetails != null)
            {
                correspondentForNoticeContact =
                    await GetProducerContact(producerBusiness.correspondentForNotices.contactDetails, context);
            }

            Company company = null;
            Partnership partnership = null;
            if (item.GetType() == typeof(companyType))
            {
                companyType companyitem = (companyType)item;

                ProducerContact registeredOfficeContact =
                    await GetProducerContact(companyitem.registeredOffice.contactDetails, context);
               
                company = new Company(companyitem.companyName, companyitem.companyNumber, registeredOfficeContact);
            }
            else if (item.GetType() == typeof(partnershipType))
            {
                partnershipType partnershipItem = (partnershipType)item;
                string partnershipName = partnershipItem.partnershipName;

                ProducerContact partnershipContact =
                    await GetProducerContact(partnershipItem.principalPlaceOfBusiness.contactDetails, context);

                List<Partner> partners = new List<Partner>();
                List<string> partnersList = partnershipItem.partnershipList.ToList();
                foreach (string name in partnersList)
                {
                    Partner partner = new Partner(name);
                    partners.Add(partner);
                }

                partnership = new Partnership(partnershipName, partnershipContact, partners);
            }
            ProducerBusiness business = new ProducerBusiness(company, partnership, correspondentForNoticeContact);
            return business;
        }

        private static async Task<ProducerContact> GetProducerContact(contactDetailsType contactDetails, WeeeContext context)
        {
            string countryName = Enum.GetName(typeof(countryType),
                contactDetails.address.country);
            var country =
                await context.Countries.SingleAsync(c => c.Name == countryName);

            ProducerAddress address =
                new ProducerAddress(contactDetails.address.primaryName,
                    contactDetails.address.secondaryName,
                    contactDetails.address.streetName,
                    contactDetails.address.town,
                    contactDetails.address.locality,
                    contactDetails.address.administrativeArea,
                    country, contactDetails.address.Item);

            ProducerContact contact =
                new ProducerContact(contactDetails.title,
                    contactDetails.forename,
                    contactDetails.surname,
                    contactDetails.phoneLandLine,
                    contactDetails.phoneMobile,
                    contactDetails.fax,
                    contactDetails.email, address);

            return contact;
        }
    }
}
