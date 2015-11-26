namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateProducerObjects
{
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using GenerateDomainObjects.DataAccess;
    using Interfaces;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Requests.Scheme.MemberRegistration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Xml.MemberRegistration;

    public class GenerateFromXml : IGenerateFromXml
    {
        private readonly IXmlConverter xmlConverter;
        private readonly IGenerateFromXmlDataAccess dataAccess;

        public GenerateFromXml(IXmlConverter xmlConverter, IGenerateFromXmlDataAccess dataAccess)
        {
            this.xmlConverter = xmlConverter;
            this.dataAccess = dataAccess;
        }

        public async Task<IEnumerable<Producer>> GenerateProducers(ProcessXMLFile messageXmlFile, MemberUpload memberUpload, Dictionary<string, ProducerCharge> producerCharges)
        {
            var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile));
            var producers = await GenerateProducerData(deserializedXml, memberUpload.SchemeId, memberUpload, producerCharges);
            return producers;
        }

        public MemberUpload GenerateMemberUpload(ProcessXMLFile messageXmlFile, List<MemberUploadError> errors, decimal totalCharges, Guid schemeId)
        {
            if (errors != null && errors.Any(e => e.ErrorType == MemberUploadErrorType.Schema))
            {
                return new MemberUpload(messageXmlFile.OrganisationId, xmlConverter.XmlToUtf8String(messageXmlFile), errors, totalCharges, null, schemeId, messageXmlFile.FileName);
            }
            else
            {
                var xml = xmlConverter.XmlToUtf8String(messageXmlFile);
                var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile));
                return new MemberUpload(messageXmlFile.OrganisationId, xml, errors, totalCharges, int.Parse(deserializedXml.complianceYear), schemeId, messageXmlFile.FileName);
            }
        }

        public async Task<IEnumerable<Producer>> GenerateProducerData(schemeType scheme, Guid schemeId, MemberUpload memberUpload, Dictionary<string, ProducerCharge> producerCharges)
        {
            List<Producer> producers = new List<Producer>();

            int numberOfPrnsNeeded = scheme.producerList.Count(p => p.status == statusType.I);
            Queue<string> generatedPrns = await dataAccess.ComputePrns(numberOfPrnsNeeded);

            foreach (producerType producerData in scheme.producerList)
            {
                var producerName = producerData.GetProducerName();

                if (producerCharges == null)
                {
                    throw new ApplicationException("No charges have been supplied");
                }
                if (!producerCharges.ContainsKey(producerName))
                {
                    throw new ApplicationException(string.Format("No charges have been supplied for the {0}.", producerName));
                }
                var chargeBandAmount = producerCharges[producerName].ChargeBandAmount;
                var chargeThisUpdate = producerCharges[producerName].Amount;

                List<BrandName> brandNames = producerData.producerBrandNames.Select(name => new BrandName(name)).ToList();

                List<SICCode> codes = producerData.SICCodeList.Select(name => new SICCode(name)).ToList();

                ProducerBusiness producerBusiness = await SetProducerBusiness(producerData.producerBusiness);

                AuthorisedRepresentative authorisedRepresentative = await SetAuthorisedRepresentative(producerData.authorisedRepresentative);

                EEEPlacedOnMarketBandType eeebandType = Enumeration.FromValue<EEEPlacedOnMarketBandType>((int)producerData.eeePlacedOnMarketBand);

                SellingTechniqueType sellingTechniqueType = Enumeration.FromValue<SellingTechniqueType>((int)producerData.sellingTechnique);

                ObligationType obligationType = producerData.obligationType.ToDomainObligationType();

                AnnualTurnOverBandType annualturnoverType = Enumeration.FromValue<AnnualTurnOverBandType>((int)producerData.annualTurnoverBand);

                DateTime? ceaseDate = null;
                if (producerData.ceaseToExistDateSpecified)
                {
                    ceaseDate = producerData.ceaseToExistDate;
                }

                string producerRegistrationNo = producerData.registrationNo;
                if (producerData.status == statusType.I)
                {
                    producerRegistrationNo = generatedPrns.Dequeue();
                }

                Producer producer = new Producer(schemeId,
                    memberUpload,
                    producerBusiness,
                    authorisedRepresentative,
                    SystemTime.UtcNow,
                    producerData.annualTurnover,
                    producerData.VATRegistered,
                    producerRegistrationNo,
                    ceaseDate,
                    producerData.tradingName,
                    eeebandType,
                    sellingTechniqueType,
                    obligationType,
                    annualturnoverType,
                    brandNames,
                    codes,
                    false,
                    chargeBandAmount,
                    chargeThisUpdate);

                // modify producer data
                switch (producerData.status)
                {
                    case statusType.A:

                        // Get the producers for scheme based on producer->prn and producer->lastsubmitted
                        // is latest date and memberupload ->IsSubmitted is true.
                        var producerDb = await dataAccess.GetLatestProducerRecord(schemeId, producerRegistrationNo);
                        if (producerDb == null)
                        {
                            // Check in migrated producers list
                            var migratedProducers = await dataAccess.GetMigratedProducer(producerRegistrationNo);
                            if (migratedProducers == null)
                            {
                                // Check for producer in another scheme member uploads
                                var anotherSchemeProducerDb = await dataAccess.GetLatestProducerRecordExcludeScheme(schemeId, producerRegistrationNo);
                                if (anotherSchemeProducerDb == null)
                                {
                                    throw new InvalidOperationException(
                                        string.Format(
                                            "PRN: {0} does not exists in current data set.",
                                            producerRegistrationNo));
                                }
                                else
                                {
                                    producers.Add(producer);
                                }
                            }
                            else
                            {
                                producers.Add(producer);
                            }
                        }
                        else if (!producerDb.Equals(producer))
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

        public async Task<AuthorisedRepresentative> SetAuthorisedRepresentative(authorisedRepresentativeType representative)
        {
            AuthorisedRepresentative result = null;

            if (representative != null &&
                representative.overseasProducer != null)
            {
                ProducerContact contact = null;
                if (representative.overseasProducer.overseasContact != null)
                {
                    contact = await GetProducerContact(representative.overseasProducer.overseasContact);
                }

                result = new AuthorisedRepresentative(representative.overseasProducer.overseasProducerName, contact);
            }

            return result;
        }

        public async Task<ProducerBusiness> SetProducerBusiness(producerBusinessType producerBusiness)
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
            if (item is companyType)
            {
                companyType companyitem = (companyType)item;
                ProducerContact contact = await GetProducerContact(companyitem.registeredOffice.contactDetails);
                company = new Company(companyitem.companyName, companyitem.companyNumber, contact);
            }
            else if (item is partnershipType)
            {
                partnershipType partnershipItem = (partnershipType)item;
                string partnershipName = partnershipItem.partnershipName;

                List<string> partnersList = partnershipItem.partnershipList.ToList();
                List<Partner> partners = partnersList.Select(name => new Partner(name)).ToList();
                ProducerContact contact = await GetProducerContact(partnershipItem.principalPlaceOfBusiness.contactDetails);
                partnership = new Partnership(partnershipName, contact, partners);
            }

            return new ProducerBusiness(company, partnership, correspondentForNoticeContact);
        }

        public async Task<ProducerContact> GetProducerContact(contactDetailsType contactDetails)
        {
            var countryName = GetCountryName(contactDetails.address.country);
            var country = await dataAccess.GetCountry(countryName);

            ProducerAddress address = new ProducerAddress(
                contactDetails.address.primaryName,
                contactDetails.address.secondaryName,
                contactDetails.address.streetName,
                contactDetails.address.town,
                contactDetails.address.locality,
                contactDetails.address.administrativeArea,
                country,
                contactDetails.address.Item);

            ProducerContact contact = new ProducerContact(
                contactDetails.title,
                contactDetails.forename,
                contactDetails.surname,
                contactDetails.phoneLandLine,
                contactDetails.phoneMobile,
                contactDetails.fax,
                contactDetails.email,
                address);

            return contact;
        }

        public string GetCountryName(countryType country)
        {
            string countryName = null;

            // Read the country name from XML attribute if defined
            var countryFirstOrDefault = typeof(countryType).GetMember(country.ToString()).FirstOrDefault();
            if (countryFirstOrDefault != null)
            {
                var countryEnumAttribute = countryFirstOrDefault
                    .GetCustomAttributes(false)
                    .OfType<XmlEnumAttribute>()
                    .FirstOrDefault();
                countryName = countryEnumAttribute != null ? countryEnumAttribute.Name : countryFirstOrDefault.Name;
            }

            return countryName;
        }
    }
}
