﻿namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain;
    using Domain.Lookup;
    using Domain.Producer;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Domain.DataReturns;
    using Shared;
    using Quarter = EA.Weee.Domain.DataReturns.Quarter;
    using QuarterType = EA.Weee.Domain.DataReturns.QuarterType;
    using RandomHelper = Core.Scheme.MemberUploadTesting.RandomHelper;

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
            for (int index = 0; index < numberOfDeliveredToAatfs; ++index)
            {
                var deliveredToAatfs = CreateDeliveredToAatfs();
                foreach (var deliveredToAatf in deliveredToAatfs)
                {
                    dataReturnVersion.WeeeDeliveredReturnVersion.AddAatfDeliveredAmount(deliveredToAatf);
                }
            }

            int numberOfDeliveredToAes = settings.NumberOfAes;
            for (int index = 0; index < numberOfDeliveredToAes; ++index)
            {
                var deliveredToAes = CreateDeliveredToAes();
                foreach (var deliveredToAe in deliveredToAes)
                {
                    dataReturnVersion.WeeeDeliveredReturnVersion.AddAeDeliveredAmount(deliveredToAe);
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

            IEnumerable<RegisteredProducer> producersToInclude = registeredProducers
                .Shuffle()
                .Take(numberOfProducers);

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

        private static IEnumerable<AatfDeliveredAmount> CreateDeliveredToAatfs()
        {
            var deliveredToAatfs = new List<AatfDeliveredAmount>();

            string aatfApprovalNumber = GetRandomAtfApprovalNumber();
            string facilityName = RandomHelper.CreateRandomString("Facility", 0, 250);

            var deliveryLocation = new AatfDeliveryLocation(aatfApprovalNumber, facilityName);

            IEnumerable<IReturnItem> returnItems = CreateReturnItems(null);
            foreach (IReturnItem returnItem in returnItems)
            {
                deliveredToAatfs.Add(new AatfDeliveredAmount(returnItem.ObligationType, returnItem.WeeeCategory, returnItem.Tonnage, deliveryLocation));
            }

            return deliveredToAatfs;
        }

        private static IEnumerable<AeDeliveredAmount> CreateDeliveredToAes()
        {
            var deliveredToAes = new List<AeDeliveredAmount>();

            string approvalNumber = GetRandomAeApprovalNumber();
            string operatorName = RandomHelper.CreateRandomString("Operator", 0, 250);

            var deliveryLocation = new AeDeliveryLocation(approvalNumber, operatorName);

            IEnumerable<IReturnItem> returnItems = CreateReturnItems(null);
            foreach (IReturnItem returnItem in returnItems)
            {
                deliveredToAes.Add(new AeDeliveredAmount(returnItem.ObligationType, returnItem.WeeeCategory, returnItem.Tonnage, deliveryLocation));
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
                .Take(numberOfResults);
        }

        private static decimal GetRandomReturnAmount()
        {
            decimal amount = (decimal)(r.NextDouble() * 1000000);

            return Math.Round(amount, 3);
        }

        private static string GetRandomAtfApprovalNumber()
        {
            string letterPair1 = RandomHelper.CreateRandomString(string.Empty, 2, 2, false);
            string number = RandomHelper.CreateRandomStringOfNumbers(4, 4);
            string letterPair2 = RandomHelper.CreateRandomString(string.Empty, 2, 2, false);

            return string.Format("WEE/{0}{1}{2}/ATF", letterPair1, number, letterPair2);
        }

        private static string GetRandomAeApprovalNumber()
        {
            string letterPair1 = RandomHelper.CreateRandomString(string.Empty, 2, 2, false);
            string number = RandomHelper.CreateRandomStringOfNumbers(4, 4);
            string letterPair2 = RandomHelper.CreateRandomString(string.Empty, 2, 2, false);

            string end;
            if (RandomHelper.OneIn(2))
            {
                end = "AE";
            }
            else
            {
                end = "EXP";
            }

            return string.Format("WEE/{0}{1}{2}/{3}", letterPair1, number, letterPair2, end);
        }
    }
}
