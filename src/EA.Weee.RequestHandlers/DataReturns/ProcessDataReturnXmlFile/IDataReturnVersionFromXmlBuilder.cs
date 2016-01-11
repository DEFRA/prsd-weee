namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System.Threading.Tasks;
    using ReturnVersionBuilder;
    using Xml.DataReturns;

    /// <summary>
    /// Builds data return version domain objects for a specified scheme and quarter by processing
    /// a SchemeReturn object representing the contents of a schematically valid XML file.
    /// The scheme and quarter are determined by the IDataReturnVersionBuilder provided.
    /// </summary>
    public interface IDataReturnVersionFromXmlBuilder
    {
        Task<DataReturnVersionBuilderResult> Build(SchemeReturn schemeReturn);
    }
}
