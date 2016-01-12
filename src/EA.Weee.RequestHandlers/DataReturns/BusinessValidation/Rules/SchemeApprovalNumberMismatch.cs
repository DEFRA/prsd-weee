namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using Core.Shared;
    using Domain.Scheme;
    using ReturnVersionBuilder;

    public class SchemeApprovalNumberMismatch : ISchemeApprovalNumberMismatch
    {
        public DataReturnVersionBuilderResult Validate(string xmlSchemeApprovalNumber, Scheme scheme)
        {
            var result = new DataReturnVersionBuilderResult();

            if (xmlSchemeApprovalNumber != scheme.ApprovalNumber)
            {
                var errorMessage = string.Format(
                    "The PCS approval number {0} you have provided does not match with the PCS. Review the PCS approval number.",
                    xmlSchemeApprovalNumber);

                result.ErrorData.Add(new ErrorData(errorMessage, ErrorLevel.Error));
            }

            return result;
        }
    }
}
