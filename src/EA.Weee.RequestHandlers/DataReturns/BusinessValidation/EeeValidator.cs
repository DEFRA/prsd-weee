namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Producer;
    using ReturnVersionBuilder;
    using Shared;
    using XmlValidation.BusinessValidation;
    using ObligationType = Domain.ObligationType;
    using Scheme = Domain.Scheme.Scheme;

    public class EeeValidator : IEeeValidator
    {
        private readonly Scheme scheme;

        private readonly Quarter quarter;

        private readonly IDataReturnVersionBuilderDataAccess schemeQuarterDataAccess;

        public EeeValidator(Scheme scheme, Quarter quarter,
            Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate)
        {
            this.scheme = scheme;
            this.quarter = quarter;
            schemeQuarterDataAccess = dataAccessDelegate(scheme, quarter);
        }

        public async Task<List<ErrorData>> Validate(string producerRegistrationNumber, string producerName, 
            WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            List<ErrorData> errorsAndWarnings = new List<ErrorData>();

            errorsAndWarnings.AddIfNotDefault<ErrorData>(await CheckProducerIsRegisteredWithSchemeForYear(producerRegistrationNumber));

            //TODO: Implement further business validation rules.

            return errorsAndWarnings;
        }

        private async Task<ErrorData> CheckProducerIsRegisteredWithSchemeForYear(string producerRegistrationNumber)
        {
            RegisteredProducer producer = await schemeQuarterDataAccess.GetRegisteredProducer(producerRegistrationNumber);

            if (producer == null)
            {
                String errorMessage = string.Format(
                    "The producer with producer registration number {0} is not a registered member of your producer compliance scheme for {1}. "
                    + "Remove this producer from your return, or ensure they are a registered member of your scheme.",
                    producerRegistrationNumber,
                    quarter.Year);
                return new ErrorData(errorMessage, ErrorLevel.Error);
            }

            return null;
        }
    }
}
