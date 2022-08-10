namespace EA.Weee.Web.Infrastructure
{
    using System.IO;

    public interface IPdfDocumentProvider
    {
        MemoryStream GeneratePdfFromHtml(string htmlDocument);
    }
}