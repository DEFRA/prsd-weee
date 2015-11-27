namespace EA.Weee.XmlValidation.Errors
{
    using Core.Scheme;
    using System.Xml.Linq;

    public interface IXmlErrorTranslator
    {
        string MakeFriendlyErrorMessage(XElement sender, string message, int lineNumber, SchemaVersion schemaVersion);

        string MakeFriendlyErrorMessage(string message, SchemaVersion schemaVersion);
    }
}