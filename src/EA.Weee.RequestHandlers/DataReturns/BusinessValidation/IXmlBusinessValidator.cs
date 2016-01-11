namespace EA.Weee.RequestHandlers.DataReturns.BusinessValidation
{
    using System.Threading.Tasks;
    using ReturnVersionBuilder;
    using Xml.DataReturns;

    /// <summary>
    /// Checks business validation rules that only apply to data provided in XML.
    /// </summary>
    public interface IXmlBusinessValidator
    {
        Task<DataReturnVersionBuilderResult> Validate(SchemeReturn schemeReturn);
    }
}
