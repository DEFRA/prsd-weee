namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.Lookup;
    using ObligationType = Domain.Obligation.ObligationType;

    public interface IEeeValidator
    {
        Task<List<ErrorData>> Validate(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage);
    }
}
