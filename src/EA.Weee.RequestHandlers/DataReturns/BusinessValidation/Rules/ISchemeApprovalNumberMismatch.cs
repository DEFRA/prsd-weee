namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using Core.Shared;
    using Domain.Scheme;
    using System.Collections.Generic;

    public interface ISchemeApprovalNumberMismatch
    {
        IEnumerable<ErrorData> Validate(string xmlSchemeApprovalNumber, Scheme scheme);
    }
}
