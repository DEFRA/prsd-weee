namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BusinessValidation;
    using BusinessValidation.Rules;
    using Domain;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Prsd.Core;
    using ErrorData = Core.Shared.ErrorData;
    using ObligationType = Domain.Obligation.ObligationType;
    using Scheme = Domain.Scheme.Scheme;

    public class DataReturnVersionBuilder : IDataReturnVersionBuilder
    {
        public Scheme Scheme { get; private set; }

        public Quarter Quarter { get; private set; }

        private readonly IEeeValidator eeeValidator;
        private readonly IDataReturnVersionBuilderDataAccess schemeQuarterDataAccess;
        private readonly ISubmissionWindowClosed submissionWindowClosed;
        private readonly List<WeeeCollectedAmount> weeeCollectedAmounts;
        private readonly List<WeeeDeliveredAmount> weeeDeliveredAmounts;
        private readonly List<EeeOutputAmount> eeeOutputAmounts;

        protected List<ErrorData> Errors { get; set; }

        public DataReturnVersionBuilder(
            Scheme scheme,
            Quarter quarter,
            Func<Scheme, Quarter, Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess>, IEeeValidator> eeeValidatorDelegate,
            Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate,
            ISubmissionWindowClosed submissionWindowClosed)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);
            Guard.ArgumentNotNull(() => quarter, quarter);

            Scheme = scheme;
            Quarter = quarter;
            eeeValidator = eeeValidatorDelegate(scheme, quarter, dataAccessDelegate);
            schemeQuarterDataAccess = dataAccessDelegate(scheme, quarter);
            this.submissionWindowClosed = submissionWindowClosed;

            Errors = new List<ErrorData>();
            weeeCollectedAmounts = new List<WeeeCollectedAmount>();
            weeeDeliveredAmounts = new List<WeeeDeliveredAmount>();
            eeeOutputAmounts = new List<EeeOutputAmount>();
        }

        public Task<IEnumerable<ErrorData>> PreValidate()
        {
            return submissionWindowClosed.Validate(Quarter);
        }

        public Task AddAatfDeliveredAmount(string aatfApprovalNumber, string facilityName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            weeeDeliveredAmounts.Add(new WeeeDeliveredAmount(obligationType, category, tonnage, new AatfDeliveryLocation(aatfApprovalNumber, facilityName)));

            return Task.Delay(0);
        }

        public Task AddAeDeliveredAmount(string approvalNumber, string operatorName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
            {
            weeeDeliveredAmounts.Add(new WeeeDeliveredAmount(obligationType, category, tonnage, new AeDeliveryLocation(approvalNumber, operatorName)));

            return Task.Delay(0);
        }

        public async Task AddEeeOutputAmount(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
                {
            var validationResult = await eeeValidator.Validate(producerRegistrationNumber, producerName, category, obligationType, tonnage);

            if (ConsideredValid(validationResult))
            {
                var registeredProducer = await schemeQuarterDataAccess.GetRegisteredProducer(producerRegistrationNumber);

                eeeOutputAmounts.Add(new EeeOutputAmount(obligationType, category, tonnage, registeredProducer));
                }

            Errors.AddRange(validationResult);
            }

        public Task AddWeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            weeeCollectedAmounts.Add(new WeeeCollectedAmount(sourceType, obligationType, category, tonnage));

            return Task.Delay(0);
        }

        public async Task<DataReturnVersionBuilderResult> Build()
        {
            DataReturnVersion dataReturnVersion = null;
            List<ErrorData> uniqueErrors = Errors.Distinct().ToList();

            if (ConsideredValid(Errors))
            {
                WeeeCollectedReturnVersion weeeCollectedReturnVersion;
                WeeeDeliveredReturnVersion weeeDeliveredReturnVersion;
                EeeOutputReturnVersion eeeOutputReturnVersion;

                bool reuseLatestWeeeDeliveredReturnVerion = false;
                bool reuseLatestWeeeCollectedReturnVersion = false;
                bool reuseLatestEeeOutputReturnVersion = false;

                ICollection<WeeeCollectedAmount> mergedWeeeCollectedAmounts;
                ICollection<WeeeDeliveredAmount> mergedWeeeDeliveredAmounts;
                ICollection<EeeOutputAmount> mergedEeeOutputAmounts;

                // Retrieve the latest data return version stored in the database.
                var latestDataReturnVersion = await schemeQuarterDataAccess.GetLatestDataReturnVersionOrDefault();
                if (latestDataReturnVersion == null)
        {
                    // No latest data return version is available, therefore create new returns data.
                    mergedWeeeCollectedAmounts = weeeCollectedAmounts;
                    mergedWeeeDeliveredAmounts = weeeDeliveredAmounts;
                    mergedEeeOutputAmounts = eeeOutputAmounts;
                }
                else
                {
                    // Unchanged data from the latest data return version can be reused. Check whether all or some of them can be reused.
                    reuseLatestWeeeCollectedReturnVersion =
                        ReplaceEqualItems(weeeCollectedAmounts, latestDataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts, out mergedWeeeCollectedAmounts);

                    reuseLatestWeeeDeliveredReturnVerion =
                        ReplaceEqualItems(weeeDeliveredAmounts, latestDataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts, out mergedWeeeDeliveredAmounts);

                    reuseLatestEeeOutputReturnVersion =
                        ReplaceEqualItems(eeeOutputAmounts, latestDataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts, out mergedEeeOutputAmounts);
                }

                if (reuseLatestWeeeDeliveredReturnVerion)
                {
                    weeeDeliveredReturnVersion = latestDataReturnVersion.WeeeDeliveredReturnVersion;
                }
                else
                {
                    weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();
                    foreach (var weeeDeliveredAmount in mergedWeeeDeliveredAmounts)
            {
                        weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(weeeDeliveredAmount);
                    }
                }

                if (reuseLatestWeeeCollectedReturnVersion)
                {
                    weeeCollectedReturnVersion = latestDataReturnVersion.WeeeCollectedReturnVersion;
                }
                else
                {
                    weeeCollectedReturnVersion = new WeeeCollectedReturnVersion();
                    foreach (var weeeCollectedAmount in mergedWeeeCollectedAmounts)
                    {
                        weeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount);
                    }
            }

                if (reuseLatestEeeOutputReturnVersion)
                {
                    eeeOutputReturnVersion = latestDataReturnVersion.EeeOutputReturnVersion;
                }
                else
                {
                    eeeOutputReturnVersion = new EeeOutputReturnVersion();
                    foreach (var eeeOutputAmount in mergedEeeOutputAmounts)
                    {
                        eeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount);
                    }
        }

                var dataReturn = await schemeQuarterDataAccess.FetchDataReturnOrDefault();
                if (dataReturn == null)
        {
                    dataReturn = new DataReturn(Scheme, Quarter);
                }

                dataReturnVersion = new DataReturnVersion(dataReturn, weeeCollectedReturnVersion, weeeDeliveredReturnVersion, eeeOutputReturnVersion);
            }

            return new DataReturnVersionBuilderResult(dataReturnVersion, uniqueErrors);
        }

        private static bool ConsideredValid(ICollection<ErrorData> errorData)
            {
            return !errorData.Any(e => e.ErrorLevel == Core.Shared.ErrorLevel.Error);
            }

        private static bool ReplaceEqualItems<T>(ICollection<T> replaceableItems, ICollection<T> availableItems, out ICollection<T> result)
            where T : class, IEquatable<T>
        {
            var allItemsReplaced = false;

            if (replaceableItems.UnorderedEqual(availableItems))
            {
                // If all the items are equal, replace the entire collection
                result = availableItems;
                allItemsReplaced = true;
            }
            else if (availableItems == null)
            {
                result = replaceableItems;
        }
            else
            {
                result = new List<T>();

                foreach (var replaceableItem in replaceableItems)
        {
                    var availableItem = availableItems.FirstOrDefault(x => x.Equals(replaceableItem));
                    result.Add(availableItem ?? replaceableItem);
                }
            }

            return allItemsReplaced;
        }
    }
}
