namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BusinessValidation;
    using BusinessValidation.Rules;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Prsd.Core;
    using ObligationType = Domain.ObligationType;
    using Scheme = Domain.Scheme.Scheme;

    public class DataReturnVersionBuilder : IDataReturnVersionBuilder
    {
        public Scheme Scheme { get; private set; }

        public Quarter Quarter { get; private set; }

        private readonly IEeeValidator eeeValidator;
        private readonly IDataReturnVersionBuilderDataAccess schemeQuarterDataAccess;
        private readonly ISubmissionWindowClosed submissionWindowClosed;

        protected List<ErrorData> Errors { get; set; }

        private WeeeCollectedReturnVersion weeeCollectedReturnVersion;
        private WeeeDeliveredReturnVersion weeeDeliveredReturnVersion;
        private EeeOutputReturnVersion eeeOutputReturnVersion;

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
        }

        public Task<IEnumerable<ErrorData>> PreValidate()
        {
            return submissionWindowClosed.Validate(Quarter);
        }

        public Task AddAatfDeliveredAmount(string aatfApprovalNumber, string facilityName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            if (weeeDeliveredReturnVersion == null)
            {
                weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();
            }

            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(new WeeeDeliveredAmount(obligationType, category, tonnage, new AatfDeliveryLocation(aatfApprovalNumber, facilityName)));

            return Task.Delay(0);
        }

        public Task AddAeDeliveredAmount(string approvalNumber, string operatorName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            if (weeeDeliveredReturnVersion == null)
            {
                weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();
            }

            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(new WeeeDeliveredAmount(obligationType, category, tonnage, new AeDeliveryLocation(approvalNumber, operatorName)));

            return Task.Delay(0);
        }

        public async Task AddEeeOutputAmount(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            if (eeeOutputReturnVersion == null)
            {
                eeeOutputReturnVersion = new EeeOutputReturnVersion();
            }

            var validationResult = await eeeValidator.Validate(producerRegistrationNumber, producerName, category, obligationType, tonnage);

            if (ConsideredValid(validationResult))
            {
                var registeredProducer = await schemeQuarterDataAccess.GetRegisteredProducer(producerRegistrationNumber);

                eeeOutputReturnVersion.AddEeeOutputAmount(new EeeOutputAmount(obligationType, category, tonnage, registeredProducer));
            }

            Errors.AddRange(validationResult);
        }

        public Task AddWeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            if (weeeCollectedReturnVersion == null)
            {
                weeeCollectedReturnVersion = new WeeeCollectedReturnVersion();
            }

            weeeCollectedReturnVersion.AddWeeeCollectedAmount(new WeeeCollectedAmount(sourceType, obligationType, category, tonnage));

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

                dataReturnVersion = new DataReturnVersion(dataReturn,
                    weeeCollectedReturnVersion, weeeDeliveredReturnVersion, eeeOutputReturnVersion);
            }

            return new DataReturnVersionBuilderResult(dataReturnVersion, uniqueErrors);
        }

        private static bool ConsideredValid(ICollection<ErrorData> errorData)
        {
            return !errorData.Any(e => e.ErrorLevel == ErrorLevel.Error);
        }
    }
}
