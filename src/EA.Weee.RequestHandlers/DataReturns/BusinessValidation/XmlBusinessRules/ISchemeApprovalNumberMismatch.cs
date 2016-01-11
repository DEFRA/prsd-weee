namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.XmlBusinessRules
{
    using Domain.Scheme;
    using ReturnVersionBuilder;
    using Xml.DataReturns;

    public interface ISchemeApprovalNumberMismatch
    {
        DataReturnVersionBuilderResult Validate(SchemeReturn xmlSchemeReturn, Scheme scheme);
    }
}
