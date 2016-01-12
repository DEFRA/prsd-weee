namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.Rules
{
    using Domain.Scheme;
    using ReturnVersionBuilder;

    public interface ISchemeApprovalNumberMismatch
    {
        DataReturnVersionBuilderResult Validate(string xmlSchemeApprovalNumber, Scheme scheme);
    }
}
