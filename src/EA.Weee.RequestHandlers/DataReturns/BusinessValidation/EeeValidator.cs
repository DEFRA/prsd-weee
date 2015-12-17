namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.Lookup;
    using ReturnVersionBuilder;
    using ObligationType = Domain.ObligationType;

    public class EeeValidator : IEeeValidator
    {
        public List<ErrorData> Validate(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage)
        {
            //TODO: Implement business validation rules.
            return new List<ErrorData>();
        }
    }
}
