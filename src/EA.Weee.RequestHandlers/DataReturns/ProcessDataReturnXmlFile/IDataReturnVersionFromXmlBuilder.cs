namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Threading.Tasks;
    using ReturnVersionBuilder;
    using Xml.DataReturns;

    public interface IDataReturnVersionFromXmlBuilder
    {
        Task<DataReturnVersionBuilderResult> Build(SchemeReturn schemeReturn);
    }
}
