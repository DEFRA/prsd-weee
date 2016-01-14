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
    using ErrorLevel = Core.Shared.ErrorLevel;
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

            RegisteredProducer producer = await schemeQuarterDataAccess.GetRegisteredProducer(producerRegistrationNumber);

            // If producer is null, add an error as it is not registered with the current scheme for the compliance year.
            if (producer == null)
            {
                var errorMessage = string.Format(
                    "The producer with producer registration number {0} is not a registered member of your producer compliance scheme for {1}. "
                    + "Remove this producer from your return, or ensure they are a registered member of your scheme.",
                    producerRegistrationNumber,
                    quarter.Year);

                errorsAndWarnings.Add(new ErrorData(errorMessage, ErrorLevel.Error));
            }
            else
            {
                errorsAndWarnings.AddIfNotDefault(ValidateProducerName(producer, producerRegistrationNumber, producerName));
                errorsAndWarnings.AddIfNotDefault(ValidateProducerObligationType(producer, producerRegistrationNumber, producerName, obligationType));
            }

            return errorsAndWarnings;
        }

        public ErrorData ValidateProducerName(RegisteredProducer producer, string producerRegistrationNumber, string producerName)
        {
            ErrorData error = null;

            if (!string.Equals(producer.CurrentSubmission.OrganisationName, producerName, StringComparison.InvariantCultureIgnoreCase))
            {
                var errorMessage = string.Format(
                "The producer name {0} registered for producer registration number {1} for {2} does not match the registered producer name of {3}. Ensure the registration number and producer name match the registered details.",
                producerName,
                producerRegistrationNumber,
                quarter.Year,
                producer.CurrentSubmission.OrganisationName);

                error = new ErrorData(errorMessage, ErrorLevel.Error);
            }

            return error;
        }

        public ErrorData ValidateProducerObligationType(RegisteredProducer producer, string producerRegistrationNumber, string producerName, ObligationType obligationType)
        {
            ErrorData error = null;

            if (!producer.CurrentSubmission.ObligationType.HasFlag(obligationType))
            {
                var errorMessage = string.Format(
                "Producer registration number {0} {1} has submitted a {2} return for one or more categories but is only registered for {3}. Amend the return or submit an update for the producer's details.",
                producerRegistrationNumber,
                producerName,
                obligationType,
                producer.CurrentSubmission.ObligationType);

                error = new ErrorData(errorMessage, ErrorLevel.Error);
            }

            return error;
        }
    }
}
