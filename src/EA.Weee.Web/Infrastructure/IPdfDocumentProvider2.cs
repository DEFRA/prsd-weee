namespace EA.Weee.Web.Infrastructure
{
    using System.IO;

    public interface IPdfDocumentProvider2
    {
        byte[] GeneratePdfFromHtml(string htmlDocument);
    }
}