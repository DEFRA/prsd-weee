namespace EA.Weee.XmlValidation.Errors
{
    using System.Xml.Linq;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;

    public interface IXmlErrorTranslator
    {
        string MakeFriendlyErrorMessage(XElement sender, string message, int lineNumber, SchemaVersion schemaVersion);

        string MakeFriendlyErrorMessage(string message, SchemaVersion schemaVersion);
    }
}