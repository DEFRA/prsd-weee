namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using System.Collections.Generic;
    using Core.Shared;
    using Domain.Scheme;

    public interface ISchemeApprovalNumberMismatch
    {
        IEnumerable<ErrorData> Validate(string xmlSchemeApprovalNumber, Scheme scheme);
    }
}
