namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using EA.Weee.Domain.DataReturns;
    using System.Xml.Linq;

    public interface IXmlGenerator
    {
        XDocument GenerateXml(DataReturnVersion dataReturnVersion);
    }
}
