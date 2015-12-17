namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using ReturnVersionBuilder;
    using Xml.DataReturns;

    public interface IDataReturnFromXmlBuilder
    {
        DataReturnVersionBuilderResult Build(SchemeReturn schemeReturn);
    }
}
