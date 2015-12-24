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

            errorsAndWarnings.AddRange(await CheckProducerDetails(producerRegistrationNumber));

            //TODO: Implement further business validation rules.

            return errorsAndWarnings;
        }

        private async Task<List<ErrorData>> CheckProducerDetails(string producerRegistrationNumber)
        {
            var errors = new List<ErrorData>();

            RegisteredProducer producer = await schemeQuarterDataAccess.GetRegisteredProducer(producerRegistrationNumber);

            // If producer is null, add an error as it is not registered with the current scheme for the compliance year.
            if (producer == null)
            {
                var errorMessage = string.Format(
                    "The producer with producer registration number {0} is not a registered member of your producer compliance scheme for {1}. "
                    + "Remove this producer from your return, or ensure they are a registered member of your scheme.",
                    producerRegistrationNumber,
                    quarter.Year);
                errors.Add(new ErrorData(errorMessage, ErrorLevel.Error));
            }

            //The producer name[producer name from XML] registered for producer registration number[PRN from XML] for [compliance year from XML] does not match the registered producer name of[producer name from database].Ensure the registration number and producer name match the registered details.

            return errors;
        }
    }
}
