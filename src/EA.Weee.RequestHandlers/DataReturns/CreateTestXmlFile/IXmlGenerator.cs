namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System.Xml.Linq;
    using EA.Weee.Domain.DataReturns;

    public interface IXmlGenerator
    {
        XDocument GenerateXml(DataReturnVersion dataReturnVersion);
    }
}
