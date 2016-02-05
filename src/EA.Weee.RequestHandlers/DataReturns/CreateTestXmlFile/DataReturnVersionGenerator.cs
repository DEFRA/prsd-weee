namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Domain;
    using Domain.Obligation;
    using Domain.Producer;
    using EA.Weee.Domain.DataReturns;
    using Shared;
    using Quarter = EA.Weee.Domain.DataReturns.Quarter;
    using QuarterType = EA.Weee.Domain.DataReturns.QuarterType;
    using RandomHelper = Core.Scheme.MemberUploadTesting.RandomHelper;
    using WeeeCategory = Domain.Lookup.WeeeCategory;

    public class DataReturnVersionGenerator : IDataReturnVersionGenerator
    {
        private readonly IDataReturnVersionGeneratorDataAccess dataAccess;
        private static readonly Random r = new Random();

        public DataReturnVersionGenerator(IDataReturnVersionGeneratorDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<DataReturnVersion> GenerateAsync(TestFileSettings settings)
        {
            if (settings.NumberOfAatfs < 0 || settings.NumberOfAatfs > 250)
            {
                throw new ArgumentOutOfRangeException("settings", "The number of AATFs specified in the settings number be in the range [0, 250].");
            }

            if (settings.NumberOfAes < 0 || settings.NumberOfAes > 50)
            {
                throw new ArgumentOutOfRangeException("settings", "The number of AEs specified in the settings number be in the range [0, 50].");
            }

            Domain.Scheme.Scheme scheme = await dataAccess.FetchSchemeAsync(settings.OrganisationID);

            Quarter quarter = new Quarter(
                settings.Quarter.Year,
                (QuarterType)settings.Quarter.Q);

            DataReturn dataReturn = new DataReturn(scheme, quarter);

            DataReturnVersion dataReturnVersion = new DataReturnVersion(dataReturn);

            IEnumerable<ReturnItem> returnItemsCollectedFromDcf = CreateReturnItems(null);
            foreach (var returnItem in returnItemsCollectedFromDcf)
            {
                dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                    new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf,
                                            returnItem.ObligationType,
                                            returnItem.WeeeCategory,
                                            returnItem.Tonnage));
            }

            int numberOfDeliveredToAatfs = settings.NumberOfAatfs;
            int aatfApprovalNumberSeedOffset = r.Next(250);

            List<string> aatfApprovalNumbers = new List<string>();
            for (int index = 0; index < numberOfDeliveredToAatfs; ++index)
            {
                int approvalNumberSeed = (index + aatfApprovalNumberSeedOffset) % 250;
                aatfApprovalNumbers.Add(GetAtfApprovalNumber(approvalNumberSeed));
            }

            IOrderedEnumerable<string> orderedAatfApprovalNumbers = aatfApprovalNumbers.OrderBy(x => x);

            foreach (string approvalNumber in orderedAatfApprovalNumbers)
            {
                var deliveredToAatfs = CreateDeliveredToAatfs(approvalNumber);
                foreach (var deliveredToAatf in deliveredToAatfs)
                {
                    dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(deliveredToAatf);
                }
            }

            int numberOfDeliveredToAes = settings.NumberOfAes;
            int aaeApprovalNumberSeedOffset = r.Next(50);

            List<string> aaeApprovalNumbers = new List<string>();
            for (int index = 0; index < numberOfDeliveredToAes; ++index)
            {
                int approvalNumberSeed = (index + aaeApprovalNumberSeedOffset) % 250;
                aaeApprovalNumbers.Add(GetAeApprovalNumber(approvalNumberSeed));
            }

            IOrderedEnumerable<string> orderedAaeApprovalNumbers = aaeApprovalNumbers.OrderBy(x => x);

            foreach (string approvalNumber in orderedAaeApprovalNumbers)
            {
                var deliveredToAes = CreateDeliveredToAes(approvalNumber);
                foreach (var deliveredToAe in deliveredToAes)
                {
                    dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(deliveredToAe);
                }
            }

            IEnumerable<ReturnItem> b2cWeeeFromDistributors = CreateReturnItems(ObligationType.B2C);
            foreach (var returnItem in b2cWeeeFromDistributors)
            {
                dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                    new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor,
                                            returnItem.ObligationType,
                                            returnItem.WeeeCategory,
                                            returnItem.Tonnage));
            }

            IEnumerable<ReturnItem> b2cWeeeFromFinalHolders = CreateReturnItems(ObligationType.B2C);
            foreach (var returnItem in b2cWeeeFromFinalHolders)
            {
                dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(
                    new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder,
                                            returnItem.ObligationType,
                                            returnItem.WeeeCategory,
                                            returnItem.Tonnage));
            }

            IList<RegisteredProducer> registeredProducers = await dataAccess.FetchRegisteredProducersAsync(scheme, quarter.Year);

            int numberOfProducers;
            if (settings.AllProducers)
            {
                numberOfProducers = registeredProducers.Count;
            }
            else
            {
                numberOfProducers = Math.Min(settings.NumberOfProduces, registeredProducers.Count);
            }

            IOrderedEnumerable<RegisteredProducer> producersToInclude = registeredProducers
                .Shuffle()
                .Take(numberOfProducers)
                .OrderBy(x => x.ProducerRegistrationNumber);

            foreach (RegisteredProducer producerToInclude in producersToInclude)
            {
                var eeeOutputAmounts = CreateEeeOutputAmounts(producerToInclude);

                foreach (var eeeOutputAmount in eeeOutputAmounts)
                {
                    dataReturnVersion.EeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount);
                }
            }

            return dataReturnVersion;
        }

        private static IEnumerable<WeeeDeliveredAmount> CreateDeliveredToAatfs(string approvalNumber)
        {
            var deliveredToAatfs = new List<WeeeDeliveredAmount>();

            string facilityName = string.Empty;
            if (RandomHelper.OneIn(2))
            {
                facilityName = RandomHelper.CreateRandomString("Facility", 0, 250);
            }

            var deliveryLocation = new AatfDeliveryLocation(approvalNumber, facilityName);

            IEnumerable<IReturnItem> returnItems = CreateReturnItems(null);
            foreach (IReturnItem returnItem in returnItems)
            {
                deliveredToAatfs.Add(new WeeeDeliveredAmount(returnItem.ObligationType, returnItem.WeeeCategory, returnItem.Tonnage, deliveryLocation));
            }

            return deliveredToAatfs;
        }

        private static IEnumerable<WeeeDeliveredAmount> CreateDeliveredToAes(string approvalNumber)
        {
            var deliveredToAes = new List<WeeeDeliveredAmount>();

            string operatorName = string.Empty;
            if (RandomHelper.OneIn(2))
            {
                operatorName = RandomHelper.CreateRandomString("Operator", 0, 250);
            }

            var deliveryLocation = new AeDeliveryLocation(approvalNumber, operatorName);

            IEnumerable<IReturnItem> returnItems = CreateReturnItems(null);
            foreach (IReturnItem returnItem in returnItems)
            {
                deliveredToAes.Add(new WeeeDeliveredAmount(returnItem.ObligationType, returnItem.WeeeCategory, returnItem.Tonnage, deliveryLocation));
            }

            return deliveredToAes;
        }

        private static IEnumerable<EeeOutputAmount> CreateEeeOutputAmounts(RegisteredProducer registeredProducer)
        {
            ObligationType obligationType = registeredProducer.CurrentSubmission.ObligationType;
            IEnumerable<IReturnItem> returnItems = CreateReturnItems(obligationType);

            return returnItems.Select(x => new EeeOutputAmount(x.ObligationType, x.WeeeCategory, x.Tonnage, registeredProducer));
        }

        private static IEnumerable<ReturnItem> CreateReturnItems(ObligationType? filter)
        {
            List<ReturnItem> returnItems = new List<ReturnItem>();
            foreach (WeeeCategory category in Enum.GetValues(typeof(WeeeCategory)))
            {
                switch (filter)
                {
                    case null:
                    case ObligationType.Both:
                        returnItems.Add(new ReturnItem(ObligationType.B2B, category, GetRandomReturnAmount()));
                        returnItems.Add(new ReturnItem(ObligationType.B2C, category, GetRandomReturnAmount()));
                        break;

                    case ObligationType.B2B:
                        returnItems.Add(new ReturnItem(ObligationType.B2B, category, GetRandomReturnAmount()));
                        break;

                    case ObligationType.B2C:
                        returnItems.Add(new ReturnItem(ObligationType.B2C, category, GetRandomReturnAmount()));
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            int numberOfResults = 1 + r.Next(returnItems.Count - 1);

            return returnItems
                .Shuffle()
                .Take(numberOfResults)
                .OrderBy(ri => ri.ObligationType)
                .ThenBy(ri => ri.WeeeCategory);
        }

        private static decimal GetRandomReturnAmount()
        {
            decimal amount = (decimal)(r.NextDouble() * 1000000);

            return Math.Round(amount, 3);
        }

        /// <summary>
        /// Deterministically creates a random-looking AATF approval number
        /// using a given seed. The same will always produce the same result.
        /// </summary>
        /// <param name="seed">The non-negative seed value for the calculation.</param>
        /// <returns></returns>
        private static string GetAtfApprovalNumber(int seed)
        {
            if (seed < 0)
            {
                throw new ArgumentOutOfRangeException("seed");
            }

            char letter1 = LetterFromSeed(seed, 7, 15);
            char letter2 = LetterFromSeed(seed, 5, 11);
            int number = ((seed + 511) * (seed + 739) * 17) % 10000;
            char letter3 = LetterFromSeed(seed, 9, 17);
            char letter4 = LetterFromSeed(seed, 13, 3);

            return string.Format("WEE/{0}{1}{2:D4}{3}{4}/ATF", letter1, letter2, number, letter3, letter4);
        }

        /// <summary>
        /// Deterministically creates a random-looking AE approval number
        /// using a given seed. The same will always produce the same result.
        /// </summary>
        /// <param name="seed">The non-negative seed value for the calculation.</param>
        /// <returns></returns>
        private static string GetAeApprovalNumber(int seed)
        {
            if (seed < 0)
            {
                throw new ArgumentOutOfRangeException("seed");
            }

            char letter1 = LetterFromSeed(seed, 3, 17);
            char letter2 = LetterFromSeed(seed, 19, 3);
            int number = ((seed + 513) * (seed + 741) * 19) % 10000;
            char letter3 = LetterFromSeed(seed, 7, 21);
            char letter4 = LetterFromSeed(seed, 15, 7);

            string end;
            if (seed % 2 == 0)
            {
                end = "AE";
            }
            else
            {
                end = "EXP";
            }

            return string.Format("WEE/{0}{1}{2:D4}{3}{4}/{5}", letter1, letter2, number, letter3, letter4, end);
        }

        /// <summary>
        /// Deterministically returns a seemingly random capital letter using a given
        /// seed. The two offset values allow different sequences to be generated.
        /// </summary>
        /// <param name="seed">The non-negative seed value for the calculation.</param>
        /// <param name="offset1">A positive offset.</param>
        /// <param name="offset2">A positive offset. Ideally this offset should not share any factors with 26.</param>
        /// <returns></returns>
        private static char LetterFromSeed(int seed, int offset1, int offset2)
        {
            if (seed < 0)
            {
                throw new ArgumentOutOfRangeException("seed");
            }

            if (offset1 <= 0)
            {
                throw new ArgumentOutOfRangeException("offset1");
            }

            if (offset2 <= 0)
            {
                throw new ArgumentOutOfRangeException("offset2");
            }

            return NumberToLetter(((seed + offset1) * offset2) % 26);
        }

        /// <summary>
        /// Converts a number in the range [0, 25] to a capital letter as follows:
        /// 0 => 'A'
        /// 1 => 'B'
        /// etc.
        /// </summary>
        /// <param name="value">A number in the range [0, 25].</param>
        /// <returns></returns>
        private static char NumberToLetter(int value)
        {
            if (value < 0 || value > 25)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            return (char)(value + 65);
        }
    }
}
