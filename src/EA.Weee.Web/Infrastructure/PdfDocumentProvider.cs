namespace EA.Weee.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using iTextSharp.tool.xml;
    using iTextSharp.tool.xml.html;
    using iTextSharp.tool.xml.parser;
    using iTextSharp.tool.xml.pipeline.css;
    using iTextSharp.tool.xml.pipeline.end;
    using iTextSharp.tool.xml.pipeline.html;

    public class PdfDocumentProvider : IPdfDocumentProvider
    {
        public MemoryStream GeneratePdfFromHtml(string htmlDocument)
        {
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

            var stream = new MemoryStream();

            using (var ms = new MemoryStream())
            {
                //Create an iTextSharp Document which is an abstraction of a PDF but **NOT** a PDF
                using (var doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10))
                {
                    //Create a writer that's bound to our PDF abstraction and our stream
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        writer.CloseStream = false;

                        doc.Open();
                        doc.NewPage();

                        var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                        var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                        var context = new HtmlPipelineContext(null);
                        context.SetTagFactory(tagProcessors);
                        var htmlPipeline = new HtmlPipeline(context, new PdfWriterPipeline(doc, writer));
                        var cssPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                        var worker = new XMLWorker(cssPipeline, true);
                        var xmlParser = new XMLParser(true, worker, Encoding.UTF8);
                        using (var sr = new StringReader(htmlDocument))
                        {
                            xmlParser.Parse(sr);
                            doc.Close();
                            ms.Position = 0;
                            ms.CopyTo(stream);
                            stream.Position = 0;
                        }
                    }
                }
            }
          
            return stream;
        }
    }
}