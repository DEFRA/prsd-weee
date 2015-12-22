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
        /// <summary>
        /// Checks business validation rules that only apply to data provided in XML.
        /// </summary>
        /// <param name="schemeReturn">The contents of the XML file to be validated.</param>
        /// <param name="errorMessage">A description of the first error encountered, or null if all checks pass.</param>
        /// <returns>Returns true if all checks pass.</returns>
        bool CheckXmlBusinessValidation(SchemeReturn schemeReturn, out string errorMessage);

        Task<DataReturnVersionBuilderResult> Build(SchemeReturn schemeReturn);
    }
}
