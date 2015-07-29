namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.GenerateProducerObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using DataAccess;
    using Domain;
    using Domain.PCS;
    using Domain.Producer;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Requests.PCS.MemberRegistration;

    public class GenerateProducerObjectsFromXml : IGenerateFromXml
    {
        private readonly IXmlConverter xmlConverter;
        private readonly WeeeContext context;

        public GenerateProducerObjectsFromXml(IXmlConverter xmlConverter, WeeeContext context)
        {
            this.xmlConverter = xmlConverter;
            this.context = context;
        }

        public async Task<IEnumerable<Producer>> Generate(ProcessXMLFile messageXmlFile, MemberUpload memberUpload)
        {
            var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile));
            Guid schemeId = memberUpload.SchemeId.GetValueOrDefault();
            var producers = await SetProducerData(deserializedXml, schemeId, memberUpload);
            return producers;
        }

        private async Task<IEnumerable<Producer>> SetProducerData(schemeType scheme, Guid schemeId, MemberUpload memberUpload)
        {
            List<Producer> producers = new List<Producer>();
            foreach (producerType producerData in scheme.producerList)
            {
                List<BrandName> brandNames = producerData.producerBrandNames.Select(name => new BrandName(name)).ToList();

                List<SICCode> codes = producerData.SICCodeList.Select(name => new SICCode(name)).ToList();

                ProducerBusiness producerBusiness = await SetProducerBusiness(producerData.producerBusiness);

                AuthorisedRepresentative authorisedRepresentative = await SetAuthorisedRepresentative(producerData.authorisedRepresentative);

                EEEPlacedOnMarketBandType eeebandType = Enumeration.FromValue<EEEPlacedOnMarketBandType>((int)producerData.eeePlacedOnMarketBand);

                SellingTechniqueType sellingTechniqueType = Enumeration.FromValue<SellingTechniqueType>((int)producerData.sellingTechnique);

                ObligationType obligationType = Enumeration.FromValue<ObligationType>((int)producerData.obligationType);

                AnnualTurnOverBandType annualturnoverType = Enumeration.FromValue<AnnualTurnOverBandType>((int)producerData.annualTurnoverBand);

                DateTime? ceaseDate = null;
                if (producerData.ceaseToExistDateSpecified)
                {
                    ceaseDate = producerData.ceaseToExistDate;
                }

                string producerRegistrationNo = producerData.registrationNo;
                if (producerData.status == statusType.I)
                {
                    producerRegistrationNo = GenerateUniquePRN();
                }

                Producer producer = new Producer(schemeId,
                    memberUpload,
                    producerBusiness,
                    authorisedRepresentative,
                    SystemTime.UtcNow,
                    (decimal)producerData.annualTurnover,
                    producerData.VATRegistered,
                    producerRegistrationNo,
                    ceaseDate,
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
                        // get the producers for scheme based on producer->prn and producer->lastsubmitted
                        // is latest date and memberupload ->IsSubmitted is true.
                        var producerDb =
                            context.MemberUploads.Where(member => member.IsSubmitted && member.SchemeId == schemeId)
                                .SelectMany(p => p.Producers)
                                .Where(p => p.RegistrationNumber == producerRegistrationNo)
                                .OrderByDescending(p => p.LastSubmitted)
                                .FirstOrDefault();
                        if (producerDb == null)
                        {
                            //check in migrated producers list
                            var migratedProducers =
                                context.MigratedProducers.FirstOrDefault(m => m.ProducerRegistrationNumber == producerRegistrationNo);

                            if (migratedProducers == null)
                            {
                                throw new InvalidOperationException(string.Format("PRN: {0} does not exists in current data set and in migrated data set.", producerRegistrationNo));
                            }
                            else
                            {
                                producers.Add(producer);
                            }
                        }
                        else if (!producer.Equals(producerDb))
                        {
                            producers.Add(producer);
                        }
                        break;

                    case statusType.I:
                        producers.Add(producer);
                        break;
                }
            }

            return producers;
        }

        private async Task<AuthorisedRepresentative> SetAuthorisedRepresentative(authorisedRepresentativeType representative)
        {
            if (representative.overseasProducer == null)
            {
                return null;
            }
            var contacts = new List<ProducerContact>();
            if (representative.overseasProducer.overseasContact != null)
            {
                contacts.Add(await GetProducerContact(representative.overseasProducer.overseasContact));
            }
            AuthorisedRepresentative overSeasAuthorisedRepresentative =
                new AuthorisedRepresentative(representative.overseasProducer.overseasProducerName, contacts.FirstOrDefault());
            return overSeasAuthorisedRepresentative;
        }

        private async Task<ProducerBusiness> SetProducerBusiness(producerBusinessType producerBusiness)
        {
            object item = producerBusiness.Item;
            ProducerContact correspondentForNoticeContact = null;
            if (producerBusiness.correspondentForNotices.contactDetails != null)
            {
                correspondentForNoticeContact =
                    await GetProducerContact(producerBusiness.correspondentForNotices.contactDetails);
            }

            Company company = null;
            Partnership partnership = null;
            if (item.GetType() == typeof(companyType))
            {
                companyType companyitem = (companyType)item;
                ProducerContact contact = await GetProducerContact(companyitem.registeredOffice.contactDetails);
                company = new Company(companyitem.companyName, companyitem.companyNumber, contact);
            }
            else if (item.GetType() == typeof(partnershipType))
            {
                partnershipType partnershipItem = (partnershipType)item;
                string partnershipName = partnershipItem.partnershipName;

                List<string> partnersList = partnershipItem.partnershipList.ToList();
                List<Partner> partners = partnersList.Select(name => new Partner(name)).ToList();
                ProducerContact contact = await GetProducerContact(partnershipItem.principalPlaceOfBusiness.contactDetails);
                partnership = new Partnership(partnershipName, contact, partners);
            }
            ProducerBusiness business = new ProducerBusiness(company, partnership, correspondentForNoticeContact);
            return business;
        }

        private async Task<ProducerContact> GetProducerContact(contactDetailsType contactDetails)
        {
            var country = await GetCountry(contactDetails);
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

        private async Task<Country> GetCountry(contactDetailsType contactDetails)
        {
            var countrydetail = contactDetails.address.country;
            var countryName = string.Empty;
            var countryEnumType = typeof(countryType);

            //Read the country name from xml attribute if defined
            var countryFirstOrDefault = countryEnumType.GetMember(countrydetail.ToString()).FirstOrDefault();
            if (countryFirstOrDefault != null)
            {
                var countryEnumAttribute = countryFirstOrDefault
                    .GetCustomAttributes(false)
                    .OfType<XmlEnumAttribute>()
                    .FirstOrDefault();
                countryName = countryEnumAttribute != null ? countryEnumAttribute.Name : countryFirstOrDefault.Name;
            }

            Country country = null;
            if (!string.IsNullOrEmpty(countryName))
            {
                country = await context.Countries.SingleAsync(c => c.Name == countryName);
            }
            return country;
        }

        private static string GenerateUniquePRN()
        {
            //TODO: Replace temporary code with actual PRN generation ensuring no duplicates
            StringBuilder prnBuilder = new StringBuilder();
            prnBuilder.Append("WEE/");
            prnBuilder.Append(GetRandomLetters(2));
            prnBuilder.Append(DateTime.UtcNow.ToString("mmff"));
            prnBuilder.Append(GetRandomLetters(2));
            return prnBuilder.ToString();
        }

        private static string GetRandomLetters(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, 2)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }
    }
}
