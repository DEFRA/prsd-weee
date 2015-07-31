namespace EA.Weee.RequestHandlers.PCS.MemberUploadTesting
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.PCS.MemberUploadTesting;
    using EA.Weee.Requests.PCS.MemberUploadTesting;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Handles requests to generate XML files that can be used to test the PCS
    /// member upload functionality.
    /// </summary>
    internal class GeneratePcsXmlFileHandler : IRequestHandler<GeneratePcsXmlFile, PcsXmlFile>
    {
        private IProducerListFactory producerListFactory;
        private IXmlGenerator xmlGenerator;

        public GeneratePcsXmlFileHandler(IProducerListFactory producerListFactory, IXmlGenerator xmlGenerator)
        {
            this.producerListFactory = producerListFactory;
            this.xmlGenerator = xmlGenerator;
        }

        public async Task<PcsXmlFile> HandleAsync(GeneratePcsXmlFile message)
        {
            ProducerList producerList = await producerListFactory.CreateAsync(message.Settings);

            XDocument xml = xmlGenerator.GenerateXml(producerList, message.Settings);

            string fileName = string.Format("PCS Member Upload XML - {0:yyyy MM dd HH mm ss}.xml", SystemTime.UtcNow);

            byte[] data;

            if (message.Settings.IncludeMalformedSchema)
            {
                string badXml;
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (BrokenXmlWriter xmlWriter = new BrokenXmlWriter(stringWriter, "scheme"))
                    {
                        xml.Save(xmlWriter);
                    }
                    badXml = stringWriter.ToString();
                }
                data = Encoding.UTF8.GetBytes(badXml);
            }
            else
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    xml.Save(stream);
                    data = stream.ToArray();
                }
            }

            PcsXmlFile file = new PcsXmlFile(fileName, data);

            return file;
        }
    }
}
