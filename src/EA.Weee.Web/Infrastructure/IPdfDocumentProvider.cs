namespace EA.Weee.Web.Infrastructure
{
    using System;
    using iText.Kernel.Pdf;

    public interface IPdfDocumentProvider
    {
        byte[] GeneratePdfFromHtml(string htmlDocument, Action<PdfWriter, PdfDocument> configureSettings = null);
    }
}