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
    using ObligationType = Domain.Obligation.ObligationType;
    using Scheme = Domain.Scheme.Scheme;

    public class DataReturnVersionBuilder : IDataReturnVersionBuilder
    {
        public Scheme Scheme { get; private set; }

        public Quarter Quarter { get; private set; }

        private readonly IEeeValidator eeeValidator;
        private readonly IDataReturnVersionBuilderDataAccess schemeQuarterDataAccess;
        private readonly ISubmissionWindowClosed submissionWindowClosed;

        protected List<ErrorData> Errors { get; set; }

        private DataReturnVersion dataReturnVersion;

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

        public async Task<IEnumerable<ErrorData>> PreValidate()
        {
            return await submissionWindowClosed.Validate(Quarter);
        }

        private async Task CreateDataReturnVersion()
        {
            if (dataReturnVersion == null)
            {
                var dataReturn = await schemeQuarterDataAccess.FetchDataReturnOrDefault();
                if (dataReturn == null)
                {
                    dataReturn = new DataReturn(Scheme, Quarter);
                }

                dataReturnVersion = new DataReturnVersion(dataReturn);
            }
        }

        public async Task AddAatfDeliveredAmount(string aatfApprovalNumber, string facilityName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            await CreateDataReturnVersion();

            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(new WeeeDeliveredAmount(obligationType, category, tonnage, new AatfDeliveryLocation(aatfApprovalNumber, facilityName)));
        }

        public async Task AddAeDeliveredAmount(string approvalNumber, string operatorName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            await CreateDataReturnVersion();

            dataReturnVersion.WeeeDeliveredReturnVersion.AddWeeeDeliveredAmount(new WeeeDeliveredAmount(obligationType, category, tonnage, new AeDeliveryLocation(approvalNumber, operatorName)));
        }

        public async Task AddEeeOutputAmount(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            await CreateDataReturnVersion();

            var validationResult = await eeeValidator.Validate(producerRegistrationNumber, producerName, category, obligationType, tonnage);

            if (ConsideredValid(validationResult))
            {
                var registeredProducer = await schemeQuarterDataAccess.GetRegisteredProducer(producerRegistrationNumber);

                dataReturnVersion.EeeOutputReturnVersion.AddEeeOutputAmount(new EeeOutputAmount(obligationType, category, tonnage, registeredProducer));
            }

            Errors.AddRange(validationResult);
        }

        public async Task AddWeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            await CreateDataReturnVersion();

            dataReturnVersion.WeeeCollectedReturnVersion.AddWeeeCollectedAmount(new WeeeCollectedAmount(sourceType, obligationType, category, tonnage));
        }

        public DataReturnVersionBuilderResult Build()
        {
            if (dataReturnVersion == null)
            {
                throw new InvalidOperationException("Return data has not been provided.");
            }

            List<ErrorData> uniqueErrors = Errors.Distinct().ToList();

            return new DataReturnVersionBuilderResult(ConsideredValid(Errors) ? dataReturnVersion : null, uniqueErrors);
        }

        private static bool ConsideredValid(ICollection<ErrorData> errorData)
        {
            return !errorData.Any(e => e.ErrorLevel == ErrorLevel.Error);
        }
    }
}
