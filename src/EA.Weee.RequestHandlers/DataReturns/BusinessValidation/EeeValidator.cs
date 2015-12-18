namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using ReturnVersionBuilder;
    using ObligationType = Domain.ObligationType;
    using Scheme = Domain.Scheme.Scheme;

    public class EeeValidator : IEeeValidator
    {
        private readonly Scheme scheme;

        private readonly Quarter quarter;

        private readonly IDataReturnVersionBuilderDataAccess dataAccess;

        public EeeValidator(Scheme scheme, Quarter quarter,
            Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate)
        {
            this.scheme = scheme;
            this.quarter = quarter;
            dataAccess = dataAccessDelegate(scheme, quarter);
        }

        public async Task<List<ErrorData>> Validate(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            //TODO: Implement business validation rules.
            return new List<ErrorData>();
        }
    }
}
