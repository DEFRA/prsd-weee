namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;

    public interface ISubmissionWindowClosed
    {
        Task<IEnumerable<ErrorData>> Validate(Quarter quarter);
    }
}
