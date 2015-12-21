namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateProducerObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using GenerateDomainObjects.DataAccess;
    using Interfaces;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Requests.Scheme.MemberRegistration;
    using Xml.Converter;
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

        public async Task<IEnumerable<ProducerSubmission>> GenerateProducers(ProcessXmlFile messageXmlFile, MemberUpload memberUpload, Dictionary<string, ProducerCharge> producerCharges)
        {
            var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile.Data));
            var producers = await GenerateProducerData(deserializedXml, memberUpload, producerCharges);
            return producers;
        }

        public MemberUpload GenerateMemberUpload(ProcessXmlFile messageXmlFile, List<MemberUploadError> errors, decimal totalCharges, Scheme scheme)
        {
            if (errors != null && errors.Any(e => e.ErrorType == UploadErrorType.Schema))
            {
                return new MemberUpload(messageXmlFile.OrganisationId, xmlConverter.XmlToUtf8String(messageXmlFile.Data), errors, totalCharges, null, scheme, messageXmlFile.FileName);
            }
            else
            {
                var xml = xmlConverter.XmlToUtf8String(messageXmlFile.Data);
                var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile.Data));
                return new MemberUpload(messageXmlFile.OrganisationId, xml, errors, totalCharges, int.Parse(deserializedXml.complianceYear), scheme, messageXmlFile.FileName);
            }
        }

        public async Task<IEnumerable<ProducerSubmission>> GenerateProducerData(schemeType scheme, MemberUpload memberUpload, Dictionary<string, ProducerCharge> producerCharges)
        {
            if (memberUpload.ComplianceYear == null)
            {
                string errorMessage = "Producers cannot be generated for a member upload "
                    + "that does not have a compliance year.";
                throw new InvalidOperationException(errorMessage);
            }

            List<ProducerSubmission> producers = new List<ProducerSubmission>();

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

                RegisteredProducer registeredProducer = null;

                string producerRegistrationNo;
                switch (producerData.status)
                {
                    case statusType.I:
                    producerRegistrationNo = generatedPrns.Dequeue();
                        break;

                    case statusType.A:
                        producerRegistrationNo = producerData.registrationNo;

                        await EnsureProducerRegistrationNumberExists(producerRegistrationNo);

                        registeredProducer = await dataAccess.FetchRegisteredProducerOrDefault(
                            producerRegistrationNo,
                            memberUpload.ComplianceYear.Value,
                            memberUpload.Scheme.Id);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                if (registeredProducer == null)
                {
                    registeredProducer = new RegisteredProducer(
                        producerRegistrationNo,
                        memberUpload.ComplianceYear.Value,
                        memberUpload.Scheme);
                }

                ProducerSubmission producer = new ProducerSubmission(
                    registeredProducer,
                    memberUpload,
                    producerBusiness,
                    authorisedRepresentative,
                    SystemTime.UtcNow,
                    producerData.annualTurnover,
                    producerData.VATRegistered,
                    ceaseDate,
                    producerData.tradingName,
                    eeebandType,
                    sellingTechniqueType,
                    obligationType,
                    annualturnoverType,
                    brandNames,
                    codes,
                    chargeBandAmount,
                    chargeThisUpdate);

                // modify producer data
                switch (producerData.status)
                {
                    case statusType.A:

                        if (registeredProducer.CurrentSubmission == null)
                        {
                            producers.Add(producer);
                        }
                        else
                        {
                            if (!registeredProducer.CurrentSubmission.Equals(producer))
                            {
                                producers.Add(producer);
                            }
                            else
                            {
                                /*
                                 * The producer's details are the same as the current submission for this
                                 * registration so we don't need to update them.
                                 */
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

        private async Task EnsureProducerRegistrationNumberExists(string producerRegistrationNumber)
        {
            bool producerRegistrationExists = await dataAccess.ProducerRegistrationExists(producerRegistrationNumber);

            if (!producerRegistrationExists)
            {
                bool migratedProducerExists = await dataAccess.MigratedProducerExists(producerRegistrationNumber);

                if (!migratedProducerExists)
                {
                    string errorMessage = string.Format(
                        "PRN: {0} does not exists in current data set.",
                        producerRegistrationNumber);
                    throw new InvalidOperationException(errorMessage);
                }
            }
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
