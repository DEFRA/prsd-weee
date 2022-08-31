namespace EA.Weee.Web.Infrastructure
{
    using System.IO;
    using iText.Html2pdf;
    using iText.Html2pdf.Resolver.Font;
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf;
    using iText.Layout.Font;

    public class PdfDocumentProviderVersion2 : IPdfDocumentProvider2
    {
        public byte[] GeneratePdfFromHtml(string htmlDocument)
        {
            using (var workStream = new MemoryStream())
            using (var pdfWriter = new PdfWriter(workStream))
                using (var pdfDocument = new PdfDocument(pdfWriter))
            {
                pdfDocument.SetDefaultPageSize(PageSize.A4);
                FontProvider fontProvider = new DefaultFontProvider(true, true, true);
                //fontProvider.AddDirectory("C:\\Repos\\prsd-weee\\src\\EA.Weee.Web\\bin\\Content\\govuk_frontend\\assets\\fonts");
                //fontProvider.AddFont()

                ConverterProperties properties = new ConverterProperties();
                properties.SetBaseUri("https://localhost:44300/content/"); // need this
                properties.SetFontProvider(fontProvider);
                HtmlConverter.ConvertToPdf(htmlDocument, pdfDocument, properties);
                return workStream.ToArray();
            }

            //Passes the document to a delegated function to perform some content, margin or page size manipulation
                //pdfModifier(document);

                //document.Close();

                //Returns the written-to MemoryStream containing the PDF.   
            //    return workStream.ToArray();
            //PdfWriter writer = new PdfWriter(file);
            //PdfDocument pdf = new PdfDocument(writer);
            //pdf.SetDefaultPageSize(PageSize.A4);
            //var document = HtmlConverter.ConvertToDocument(htmlDocument, pdf, converterProperties);
            //document.Close();

            //using (var document = new Document())
            //{
            //    using (var workStream = new MemoryStream())
            //    {
            //        PdfWriter writer = PdfWriter.GetInstance(document, workStream);
            //        writer.CloseStream = false;

            //        document.SetPageSize(new Rectangle(500f, 500f, 90));
            //        document.NewPage();

            //        //if (configureSettings != null)
            //        //{
            //        //    configureSettings(writer, document);
            //        //}
            //        document.Open();

            //        using (var reader = new StringReader(htmlDocument))
            //        {
            //            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, reader);

            //            document.Close();
            //            return workStream;
            //        }
            //    }
            //}
        }
    }
}