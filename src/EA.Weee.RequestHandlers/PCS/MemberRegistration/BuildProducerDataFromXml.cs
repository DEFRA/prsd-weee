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
                List<BrandName> brandNames = producerData.producerBrandNames.Select(name => new BrandName(name)).ToList();
         
                List<SICCode> codes = producerData.SICCodeList.Select(name => new SICCode(name)).ToList();
         
                ProducerBusiness producerBusiness = await SetProducerBusiness(producerData.producerBusiness, context);

                AuthorisedRepresentative authorisedRepresentative = await SetAuthorisedRepresentative(producerData.authorisedRepresentative, context);

                EEEPlacedOnMarketBandType eeebandType = Enumeration.FromValue<EEEPlacedOnMarketBandType>((int)producerData.eeePlacedOnMarketBand);
          
                SellingTechniqueType sellingTechniqueType = Enumeration.FromValue<SellingTechniqueType>((int)producerData.sellingTechnique);
        
                ObligationType obligationType = Enumeration.FromValue<ObligationType>((int)producerData.obligationType);

                AnnualTurnOverBandType annualturnoverType = Enumeration.FromValue<AnnualTurnOverBandType>((int)producerData.annualTurnoverBand);

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
                
                // modify producer data
                switch (producerData.status)
                {
                    case statusType.A:
                        // get the producers for scheme based on producer->prn and producer->lastsubmitted is latest date and memberupload ->IsSubmitted is true.
                        var memberupload =
                            await
                                context.MemberUploads.Where(member => member.IsSubmitted && member.SchemeId == schemeId)
                                    .ToListAsync();
                        if (memberupload != null && memberupload.Count > 0)
                        {
                            var producerDb =
                                memberupload.SelectMany(m => m.Producers)
                                    .OrderByDescending(p => p.LastSubmitted)
                                    .FirstOrDefault(p => p.RegistrationNumber == producerData.registrationNo);
                            //Add only if producer not found in DB
                            if (!producer.Equals(producerDb))
                            {
                                producers.Add(producer);
                            }
                        }
                        break;

                    case statusType.I:
                        producers.Add(producer);
                        break;
                }
            }

            return producers;
        }

        private static async Task<AuthorisedRepresentative> SetAuthorisedRepresentative(authorisedRepresentativeType representative, WeeeContext context)
        {
            if (representative.overseasProducer == null)
            {
                return null;
            }
            var contacts = new List<ProducerContact>();
            if (representative.overseasProducer.overseasContact != null)
            {
                foreach (contactDetailsType detail in representative.overseasProducer.overseasContact)
                {
                    contacts.Add(await GetProducerContact(detail, context));
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
                company = new Company(companyitem.companyName, companyitem.companyNumber,
                    await GetProducerContact(companyitem.registeredOffice.contactDetails, context));
            }
            else if (item.GetType() == typeof(partnershipType))
            {
                partnershipType partnershipItem = (partnershipType)item;
                string partnershipName = partnershipItem.partnershipName;
  
                List<string> partnersList = partnershipItem.partnershipList.ToList();
                List<Partner> partners = partnersList.Select(name => new Partner(name)).ToList();

                partnership = new Partnership(partnershipName, await GetProducerContact(partnershipItem.principalPlaceOfBusiness.contactDetails, context), partners);
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
