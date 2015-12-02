namespace EA.Weee.XmlValidation.Errors
{
    using System.Xml.Linq;
    using Core.Scheme;

    public interface IXmlErrorTranslator
    {
        string MakeFriendlyErrorMessage(XElement sender, string message, int lineNumber, string schemaVersion);

        string MakeFriendlyErrorMessage(string message, string schemaVersion);
    }
}