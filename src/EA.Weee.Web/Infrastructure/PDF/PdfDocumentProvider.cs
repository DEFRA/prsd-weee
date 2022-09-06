namespace EA.Weee.Web.Infrastructure.PDF
{
    using System;
    using System.IO;
    using iText.Html2pdf;
    using iText.Html2pdf.Resolver.Font;
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf;
    using iText.Layout.Font;

    public class PdfDocumentProvider : IPdfDocumentProvider
    {
        public byte[] GeneratePdfFromHtml(string htmlDocument, Action<PdfWriter, PdfDocument> configureSettings = null)
        {
            using (var workStream = new MemoryStream())
            using (var pdfWriter = new PdfWriter(workStream))
            using (var pdfDocument = new PdfDocument(pdfWriter))
            {
                pdfDocument.SetDefaultPageSize(PageSize.A4);

                configureSettings?.Invoke(pdfWriter, pdfDocument);

                FontProvider fontProvider = new DefaultFontProvider(false, 
                    false, 
                    true);

                var properties = new ConverterProperties();
                properties.SetFontProvider(fontProvider);
                HtmlConverter.ConvertToPdf(htmlDocument, pdfDocument, properties);

                return workStream.ToArray();
            }
        }
    }
}