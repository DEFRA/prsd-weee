namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using Core.Shared;
    using Domain.DataReturns;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISubmissionWindowClosed
    {
        Task<IEnumerable<ErrorData>> Validate(Quarter quarter);
    }
}
