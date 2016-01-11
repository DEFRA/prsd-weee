namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess.DataAccess;
    using Domain.DataReturns;
    using Factories;
    using ReturnVersionBuilder;
    using Shared;
    using Xml.DataReturns;
    using XmlBusinessRules;

    /// <summary>
    /// Checks business validation rules that only apply to data provided in XML.
    /// </summary>
    public class XmlBusinessValidator : IXmlBusinessValidator
    {
        private readonly IDataReturnVersionBuilder dataReturnVersionBuilder;
        private readonly ISubmissionWindowClosed submissionWindowClosed;
        private readonly ISchemeApprovalNumberMismatch schemeApprovalNumberMismatch;

        public XmlBusinessValidator(IDataReturnVersionBuilder dataReturnVersionBuilder, ISubmissionWindowClosed submissionWindowClosed, ISchemeApprovalNumberMismatch schemeApprovalNumberMismatch)
        {
            this.dataReturnVersionBuilder = dataReturnVersionBuilder;
            this.submissionWindowClosed = submissionWindowClosed;
            this.schemeApprovalNumberMismatch = schemeApprovalNumberMismatch;
        }

        public async Task<DataReturnVersionBuilderResult> Validate(SchemeReturn schemeReturn)
        {
            // This should be validated first
            var submissionWindowClosedResult = await submissionWindowClosed.Validate(schemeReturn);

            if (submissionWindowClosedResult.ErrorData.Any())
            {
                return submissionWindowClosedResult;
            }

            var schemeApprovalNumberMismatchResult = schemeApprovalNumberMismatch.Validate(schemeReturn,
                dataReturnVersionBuilder.Scheme);

            return schemeApprovalNumberMismatchResult;
        }
    }
}
