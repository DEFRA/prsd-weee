namespace EA.Weee.RequestHandlers.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BusinessValidation;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using ObligationType = Domain.ObligationType;

    public class DataReturnVersionBuilder : IDataReturnVersionBuilder
    {
        private readonly Domain.Scheme.Scheme scheme;

        private readonly Quarter quarter;

        private readonly IEeeValidator eeeValidator;

        private readonly IDataReturnVersionBuilderDataAccess dataAccess;

        protected List<ErrorData> ErrorData { get; set; }

        private DataReturnVersion dataReturnVersion;

        public DataReturnVersionBuilder(Domain.Scheme.Scheme scheme, Quarter quarter,
            IEeeValidator eeeValidator,
            Func<Domain.Scheme.Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate)
        {
            this.scheme = scheme;
            this.quarter = quarter;
            this.eeeValidator = eeeValidator;
            dataAccess = dataAccessDelegate(scheme, quarter);

            ErrorData = new List<ErrorData>();
        }

        private async Task CreateDataReturnVersion()
        {
            if (dataReturnVersion == null)
            {
                var dataReturn = await dataAccess.FetchDataReturnOrDefault();
                if (dataReturn == null)
                {
                    dataReturn = new DataReturn(scheme, quarter);
                }

                dataReturnVersion = new DataReturnVersion(dataReturn);
            }
        }

        public async Task AddAatfDeliveredAmount(string aatfApprovalNumber, string facilityName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            await CreateDataReturnVersion();

            dataReturnVersion.AddAatfDeliveredAmount(new AatfDeliveredAmount(obligationType, category, tonnage, new AatfDeliveryLocation(aatfApprovalNumber, facilityName), dataReturnVersion));
        }

        public async Task AddAeDeliveredAmount(string approvalNumber, string operatorName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            await CreateDataReturnVersion();

            dataReturnVersion.AddAeDeliveredAmount(new AeDeliveredAmount(obligationType, category, tonnage, new AeDeliveryLocation(approvalNumber, operatorName), dataReturnVersion));
        }

        public async Task AddEeeOutputAmount(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            await CreateDataReturnVersion();

            var validationResult = await eeeValidator.Validate(producerRegistrationNumber, producerName, category, obligationType, tonnage);

            if (ConsideredValid(validationResult))
            {
                var registeredProducer = await dataAccess.GetRegisteredProducer(producerRegistrationNumber);

                dataReturnVersion.AddEeeOutputAmount(new EeeOutputAmount(obligationType, category, tonnage, registeredProducer, dataReturnVersion));
            }

            ErrorData.AddRange(validationResult);
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

            return new DataReturnVersionBuilderResult(ConsideredValid(ErrorData) ? dataReturnVersion : null, ErrorData);
        }

        private static bool ConsideredValid(ICollection<ErrorData> errorData)
        {
            return !errorData.Any(e => e.ErrorLevel == ErrorLevel.Error);
        }
    }
}
