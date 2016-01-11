namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation.XmlBusinessRules
{
    using System.Threading.Tasks;
    using ReturnVersionBuilder;
    using Xml.DataReturns;

    public interface ISubmissionWindowClosed
    {
        Task<DataReturnVersionBuilderResult> Validate(SchemeReturn xmlSchemeReturn);
    }
}
