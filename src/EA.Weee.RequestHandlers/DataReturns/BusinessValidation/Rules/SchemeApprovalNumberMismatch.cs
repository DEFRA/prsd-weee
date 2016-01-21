namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using System.Collections.Generic;
    using Core.Shared;
    using Domain.Scheme;
    using ReturnVersionBuilder;

    public class SchemeApprovalNumberMismatch : ISchemeApprovalNumberMismatch
    {
        public IEnumerable<ErrorData> Validate(string xmlSchemeApprovalNumber, Scheme scheme)
        {
            var result = new List<ErrorData>();

            if (xmlSchemeApprovalNumber != scheme.ApprovalNumber)
            {
                var errorMessage = string.Format(
                    "The PCS approval number {0} you have provided does not match with the PCS. Review the PCS approval number.",
                    xmlSchemeApprovalNumber);

                result.Add(new ErrorData(errorMessage, ErrorLevel.Error));
            }

            return result;
        }
    }
}
