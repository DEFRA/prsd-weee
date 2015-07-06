namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using System.Xml.Linq;

    internal interface IXmlErrorTranslator
    {
        string MakeFriendlyErrorMessage(XElement sender, string message, int lineNumber);
    }
}