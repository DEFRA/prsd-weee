namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using Requests.DataReturns;

    public interface IGenerateFromDataReturnXml
    {
        GenerateFromDataReturnXmlResult<T> GenerateDataReturns<T>(ProcessDataReturnXmlFile message) where T : class;
    }
}
