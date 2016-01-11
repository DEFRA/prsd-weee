namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.XmlBusinessRules
{
    using System;
    using Core.Shared;
    using Domain.Scheme;
    using ReturnVersionBuilder;
    using Xml.DataReturns;

    public class SchemeApprovalNumberMismatch : ISchemeApprovalNumberMismatch
    {
        public DataReturnVersionBuilderResult Validate(SchemeReturn xmlSchemeReturn, Scheme scheme)
        {
            var result = new DataReturnVersionBuilderResult();

            if (xmlSchemeReturn.ApprovalNo != scheme.ApprovalNumber)
            {
                var errorMessage = string.Format(
                    "The PCS approval number {0} you have provided does not match with the PCS. Review the PCS approval number.",
                    xmlSchemeReturn.ApprovalNo);

                result.ErrorData.Add(new ErrorData(errorMessage, ErrorLevel.Error));
            }

            return result;
        }
    }
}
