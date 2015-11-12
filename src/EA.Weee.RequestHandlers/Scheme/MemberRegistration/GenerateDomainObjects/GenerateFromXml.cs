namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateProducerObjects
{
    using Core.Helpers.PrnGeneration;
    using DataAccess;
    using Domain;
    using Domain.Producer;
    using Domain.Scheme;
    using Interfaces;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Requests.Scheme.MemberRegistration;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Xml;
    using Xml.Schemas;

    public class GenerateFromXml : IGenerateFromXml
    {
        private readonly IXmlConverter xmlConverter;
        private readonly WeeeContext context;

        public GenerateFromXml(IXmlConverter xmlConverter, WeeeContext context)
        {
            this.xmlConverter = xmlConverter;
            this.context = context;
        }

        public async Task<IEnumerable<Producer>> GenerateProducers(ProcessXMLFile messageXmlFile, MemberUpload memberUpload, Hashtable producerCharges)
        {
            var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile));
            var producers = await SetProducerData(deserializedXml, memberUpload.SchemeId, memberUpload, producerCharges);
            return producers;
        }

        public MemberUpload GenerateMemberUpload(ProcessXMLFile messageXmlFile, List<MemberUploadError> errors, decimal totalCharges, Guid schemeId)
        {
            if (errors != null && errors.Any(e => e.ErrorType == MemberUploadErrorType.Schema))
            {
                return new MemberUpload(messageXmlFile.OrganisationId, xmlConverter.XmlToUtf8String(messageXmlFile), errors, totalCharges, null, schemeId);
            }
            else
            {
                var xml = xmlConverter.XmlToUtf8String(messageXmlFile);
                var deserializedXml = xmlConverter.Deserialize(xmlConverter.Convert(messageXmlFile));
                return new MemberUpload(messageXmlFile.OrganisationId, xml, errors, totalCharges, int.Parse(deserializedXml.complianceYear), schemeId);
            }
        }

        private async Task<IEnumerable<Producer>> SetProducerData(schemeType scheme, Guid schemeId, MemberUpload memberUpload, Hashtable producerCharges)
        {
            List<Producer> producers = new List<Producer>();

            int numberOfPrnsNeeded = scheme.producerList.Count(p => p.status == statusType.I);
            Queue<string> generatedPrns = await ComputePrns(context, numberOfPrnsNeeded);

            foreach (producerType producerData in scheme.producerList)
            {
                var producerName = producerData.GetProducerName();

                if (producerCharges == null)
                {
                    throw new ApplicationException("No charges have been supplied");
                }
                if (producerCharges[producerName] == null)
                {
                    throw new ApplicationException(string.Format("No charges have been supplied for the {0}.", producerName));
                }
                var chargeBandAmount = ((ProducerCharge)producerCharges[producerName]).ChargeBandAmount;
                var chargeThisUpdate = ((ProducerCharge)producerCharges[producerName]).ChargeBandAmount.Amount;

                List<BrandName> brandNames = producerData.producerBrandNames.Select(name => new BrandName(name)).ToList();

                List<SICCode> codes = producerData.SICCodeList.Select(name => new SICCode(name)).ToList();

                ProducerBusiness producerBusiness = await SetProducerBusiness(producerData.producerBusiness);

                AuthorisedRepresentative authorisedRepresentative = await SetAuthorisedRepresentative(producerData.authorisedRepresentative);

                EEEPlacedOnMarketBandType eeebandType = Enumeration.FromValue<EEEPlacedOnMarketBandType>((int)producerData.eeePlacedOnMarketBand);

                SellingTechniqueType sellingTechniqueType = Enumeration.FromValue<SellingTechniqueType>((int)producerData.sellingTechnique);

                ObligationType obligationType = producerData.obligationType.ToDomainObligationType(); // Enumeration.FromValue<ObligationType>((int)producerData.obligationType);

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
                    (decimal)producerData.annualTurnover,
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

                        // get the producers for scheme based on producer->prn and producer->lastsubmitted
                        // is latest date and memberupload ->IsSubmitted is true.
                        var producerDb =
                            context.MemberUploads.Where(member => member.IsSubmitted && member.SchemeId == schemeId)
                                .SelectMany(p => p.Producers)
                                .Where(p => p.RegistrationNumber == producerRegistrationNo)
                                .OrderByDescending(p => p.UpdatedDate)
                                .FirstOrDefault();
                        if (producerDb == null)
                        {
                            //check in migrated producers list
                            var migratedProducers =
                                context.MigratedProducers.FirstOrDefault(m => m.ProducerRegistrationNumber == producerRegistrationNo);

                            if (migratedProducers == null)
                            {
                                //check for producer in another scheme member uploads
                                var anotherSchemeProducerDb =
                                    context.MemberUploads.Where(
                                        member => member.IsSubmitted && member.SchemeId != schemeId)
                                        .SelectMany(p => p.Producers)
                                        .Where(p => p.RegistrationNumber == producerRegistrationNo)
                                        .OrderByDescending(p => p.UpdatedDate)
                                        .FirstOrDefault();
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

        /// <summary>
        /// Generates unique, pseudorandom PRNs with minimal database interaction.
        /// Works by:
        /// a) uniquely mapping each unsigned integer to another pseudorandom unsigned integer
        /// b) uniquely mapping each unsigned integer to a specific PRN
        /// Combining those two mappings, and using a sequential seed, we can obtain pseudorandom PRNs
        /// with assurance that we will not repeat ourselves for a very, very long time.
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="numberOfPrnsNeeded">A non-negative integer</param>
        /// <returns></returns>
        private static async Task<Queue<string>> ComputePrns(WeeeContext context, int numberOfPrnsNeeded)
        {
            bool succeeded = false;
            bool retry = false;
            IEnumerable<DbEntityEntry> staleValues = null;
            List<PrnAsComponents> generatedPrns = new List<PrnAsComponents>();
            ExceptionDispatchInfo exceptionDispatchInfo = null;

            var prnHelper = new PrnHelper(new QuadraticResidueHelper());

            try
            {
                succeeded = false;
                retry = false;

                // to avoid concurrency issues, we want to read the latest seed, 'reserve' some PRNs (figuring
                // out the resulting final seed as we go), and write the final seed back as quickly as possible
                uint originalLatestSeed = (uint)context.SystemData.Select(sd => sd.LatestPrnSeed).First();

                uint currentSeed = originalLatestSeed;
                for (int ii = 0; ii < numberOfPrnsNeeded; ii++)
                {
                    var prnFromSeed = new PrnAsComponents(currentSeed + 1);
                    generatedPrns.Add(prnFromSeed);
                    currentSeed = prnFromSeed.ToSeedValue();
                }

                // we write back the next acceptable seed to the database, for next time
                // since there are some mathematical constraints on the acceptable values
                context.SystemData.First().LatestPrnSeed = currentSeed;
                await context.SaveChangesAsync();

                succeeded = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                staleValues = ex.Entries;
                retry = true;
            }
            catch (Exception ex)
            {
                // In .NET 4.5 it is not allowed to use "await" within catch blocks; this forces us to put
                // code after the catch block. As a result of that, we don't want to throw unhandled exceptions
                // here, so we capture the dispatch info and thow it at the end of this method.
                exceptionDispatchInfo = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex);
            }

            if (succeeded)
            {
                // now we're done with the fairly time sensitive database read/write,
                // we can 'randomise' the results at our leisure
                return new Queue<string>(generatedPrns.Select(p => prnHelper.CreateUniqueRandomVersionOfPrn(p)));
            }
            else if (retry)
            {
                // If we need to rety, we are probably in a race condition with another thread.
                // To avoid retrying indefinately, we'll wait a few milliseconds to get out of sync
                // with the other thread.
                await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(100)));

                // If the database value for [LatestPrnSeed] was updated between the time we fetched the value
                // tried to update it with our new value, we will get a DbConcurrencyException.
                // To handle this we will just call this method again until it succeeds.
                // However, as dependency injection forces us to reuse the same WeeeContext, the SystemData
                // entity will already be attached to the context giving us the same stale value from when it
                // was first fetched.
                // The DbUpdateConcurrencyException gives us the ability to reload this entity from the database.
                foreach (var entry in staleValues)
                {
                    if (entry.Entity is SystemData)
                    {
                        await entry.ReloadAsync();
                    }
                }

                // Now that we have the latest value loaded, we'll try calling this method again.
                return await ComputePrns(context, numberOfPrnsNeeded);
            }
            else
            {
                // Something else bad happened and it's not possible to fix that here.
                exceptionDispatchInfo.Throw();
                throw new Exception("This will never be thrown.");
            }
        }

        private async Task<AuthorisedRepresentative> SetAuthorisedRepresentative(authorisedRepresentativeType representative)
        {
            if (representative == null)
            {
                return null;
            }

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
    }
}
