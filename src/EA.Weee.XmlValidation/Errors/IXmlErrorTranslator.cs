namespace EA.Weee.XmlValidation.Errors
{
    using System.Xml.Linq;

    public interface IXmlErrorTranslator
    {
        string MakeFriendlyErrorMessage(XElement sender, string message, int lineNumber);

        string MakeFriendlyErrorMessage(string message);
    }
}