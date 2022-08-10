namespace EA.Weee.Web.Infrastructure
{
    using System.IO;
    using iText.Html2pdf;
    using iText.Kernel.Pdf;
    using iTextSharp.text;
    using PageSize = iText.Kernel.Geom.PageSize;

    public class PdfDocumentProviderVersion2 : IPdfDocumentProvider2
    {
        public byte[] GeneratePdfFromHtml(string htmlDocument)
        {
            //var workStream = new MemoryStream();

            //using (MemoryStream memStream = new MemoryStream())
            //{
            //    WriterProperties properties = new WriterProperties();
            //    using (PdfWriter pdfWriter = new PdfWriter(memStream, properties))
            //    {
            //        pdfWriter.SetCloseStream(true);
            //        PdfDocument pdfDoc;
            //        using (pdfDoc = new PdfDocument(pdfWriter))
            //        {
            //            ConverterProperties props = new ConverterProperties();

            //            pdfDoc.SetDefaultPageSize(PageSize.LETTER);
            //            pdfDoc.SetCloseWriter(true);
            //            pdfDoc.SetCloseReader(true);
            //            pdfDoc.SetFlushUnusedObjects(true);
            //            HtmlConverter.ConvertToPdf(htmlDocument, pdfDoc, props);
            //            pdfDoc.Close();
            //        }
            //    }
            //    return workStream;
            //}

            using (var workStream = new MemoryStream())
            using (var pdfWriter = new PdfWriter(workStream))
                using (var pdfDocument = new PdfDocument(pdfWriter))
            {
                pdfDocument.SetDefaultPageSize(PageSize.A4);
                ConverterProperties properties = new ConverterProperties();
                properties.SetBaseUri("https://localhost:44300/content/");
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