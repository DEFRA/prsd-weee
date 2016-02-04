namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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

        public async Task AddAatfDeliveredAmount(string approvalNumber, string facilityName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            var aatfDeliveryLocation = await schemeQuarterDataAccess.GetOrAddAatfDeliveryLocation(approvalNumber, facilityName);

            weeeDeliveredAmounts.Add(new WeeeDeliveredAmount(obligationType, category, tonnage, aatfDeliveryLocation));
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "aeDeliveryLocation is valid.")]
        public async Task AddAeDeliveredAmount(string approvalNumber, string operatorName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            var aeDeliveryLocation = await schemeQuarterDataAccess.GetOrAddAeDeliveryLocation(approvalNumber, operatorName);

            weeeDeliveredAmounts.Add(new WeeeDeliveredAmount(obligationType, category, tonnage, aeDeliveryLocation));
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
                var dataReturn = await schemeQuarterDataAccess.FetchDataReturnOrDefault();
                if (dataReturn == null)
                {
                    dataReturn = new DataReturn(Scheme, Quarter);
                }

                DataReturnVersion latestSubmittedDataReturnVersion = dataReturn.CurrentVersion;

                var weeeCollectedReturnVersion = BuildWeeeCollectedReturnVersion(latestSubmittedDataReturnVersion);
                var weeeDeliveredReturnVersion = BuildWeeeDeliveredReturnVersion(latestSubmittedDataReturnVersion);
                var eeeOutputReturnVersion = BuildEeeOutputReturnVersion(latestSubmittedDataReturnVersion);

                dataReturnVersion = new DataReturnVersion(dataReturn, weeeCollectedReturnVersion, weeeDeliveredReturnVersion, eeeOutputReturnVersion);
            }

            return new DataReturnVersionBuilderResult(dataReturnVersion, uniqueErrors);
        }

        private WeeeCollectedReturnVersion BuildWeeeCollectedReturnVersion(DataReturnVersion submittedDataReturnVersion)
        {
            WeeeCollectedReturnVersion weeeCollectedReturnVersion = null;

            // Unchanged data from the latest submitted data return version can be reused. Check whether all or some of them can be reused.
            if (weeeCollectedAmounts.Any())
            {
                ICollection<WeeeCollectedAmount> mergedWeeeCollectedAmounts;
                bool reuseLatestWeeeCollectedReturnVersion = false;

                if (submittedDataReturnVersion != null &&
                    submittedDataReturnVersion.WeeeCollectedReturnVersion != null)
                {
                    reuseLatestWeeeCollectedReturnVersion =
                        ReuseEqualItems(weeeCollectedAmounts, submittedDataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts, out mergedWeeeCollectedAmounts);
                }
                else
                {
                    mergedWeeeCollectedAmounts = weeeCollectedAmounts;
                }

                if (reuseLatestWeeeCollectedReturnVersion)
                {
                    weeeCollectedReturnVersion = submittedDataReturnVersion.WeeeCollectedReturnVersion;
                }
                else
                {
                    weeeCollectedReturnVersion = new WeeeCollectedReturnVersion();
                    foreach (var weeeCollectedAmount in mergedWeeeCollectedAmounts)
                    {
                        weeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount);
                    }
                }
            }

            return weeeCollectedReturnVersion;
        }

        private WeeeDeliveredReturnVersion BuildWeeeDeliveredReturnVersion(DataReturnVersion submittedDataReturnVersion)
        {
            WeeeDeliveredReturnVersion weeeDeliveredReturnVersion = null;

            // Unchanged data from the latest submitted data return version can be reused. Check whether all or some of them can be reused.
            if (weeeDeliveredAmounts.Any())
            {
                ICollection<WeeeDeliveredAmount> mergedWeeeDeliveredAmounts;
                bool reuseLatestWeeeDeliveredReturnVerion = false;

                if (submittedDataReturnVersion != null &&
                    submittedDataReturnVersion.WeeeDeliveredReturnVersion != null)
                {
                    reuseLatestWeeeDeliveredReturnVerion =
                        ReuseEqualItems(weeeDeliveredAmounts, submittedDataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts, out mergedWeeeDeliveredAmounts);
                }
                else
                {
                    mergedWeeeDeliveredAmounts = weeeDeliveredAmounts;
                }

                if (reuseLatestWeeeDeliveredReturnVerion)
                {
                    weeeDeliveredReturnVersion = submittedDataReturnVersion.WeeeDeliveredReturnVersion;
                }
                else
                {
                    weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();
                    foreach (var weeeDeliveredAmount in mergedWeeeDeliveredAmounts)
                    {
                        weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(weeeDeliveredAmount);
                    }
                }
            }

            return weeeDeliveredReturnVersion;
        }

        private EeeOutputReturnVersion BuildEeeOutputReturnVersion(DataReturnVersion submittedDataReturnVersion)
        {
            EeeOutputReturnVersion eeeOutputReturnVersion = null;

            // Unchanged data from the latest submitted data return version can be reused. Check whether all or some of them can be reused.
            if (eeeOutputAmounts.Any())
            {
                bool reuseLatestEeeOutputReturnVersion = false;
                ICollection<EeeOutputAmount> mergedEeeOutputAmounts;

                if (submittedDataReturnVersion != null &&
                    submittedDataReturnVersion.EeeOutputReturnVersion != null)
                {
                    reuseLatestEeeOutputReturnVersion =
                        ReuseEqualItems(eeeOutputAmounts, submittedDataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts, out mergedEeeOutputAmounts);
                }
                else
                {
                    mergedEeeOutputAmounts = eeeOutputAmounts;
                }

                if (reuseLatestEeeOutputReturnVersion)
                {
                    eeeOutputReturnVersion = submittedDataReturnVersion.EeeOutputReturnVersion;
                }
                else
                {
                    eeeOutputReturnVersion = new EeeOutputReturnVersion();
                    foreach (var eeeOutputAmount in mergedEeeOutputAmounts)
                    {
                        eeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount);
                    }
                }
            }

            return eeeOutputReturnVersion;
        }

        private static bool ConsideredValid(ICollection<ErrorData> errorData)
        {
            return !errorData.Any(e => e.ErrorLevel == Core.Shared.ErrorLevel.Error);
        }

        private static bool ReuseEqualItems<T>(ICollection<T> newItems, ICollection<T> reusableItems, out ICollection<T> result)
            where T : class, IEquatable<T>
        {
            var allItemsReused = false;

            if (newItems == null ||
                !newItems.Any() ||
                reusableItems == null)
            {
                result = newItems;
            }
            else if (newItems.UnorderedEqual(reusableItems))
            {
                // If all the items are equal, reuse the entire collection
                result = reusableItems;
                allItemsReused = true;
            }
            else
            {
                result = new List<T>();

                foreach (var newItem in newItems)
                {
                    var reusableItem = reusableItems.FirstOrDefault(x => x.Equals(newItem));
                    result.Add(reusableItem ?? newItem);
                }
            }

            return allItemsReused;
        }
    }
}
