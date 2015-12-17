namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation
{
    using System.Collections.Generic;
    using Core.Shared;
    using Domain.Lookup;
    using ObligationType = Domain.ObligationType;

    public interface IEeeValidator
    {
        List<ErrorData> Validate(string producerRegistrationNumber, string producerName, WeeeCategory category, ObligationType obligationType, decimal tonnage);
    }
}
