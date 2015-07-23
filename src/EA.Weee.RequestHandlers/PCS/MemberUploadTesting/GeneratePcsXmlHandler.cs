namespace EA.Weee.RequestHandlers.PCS.MemberUploadTesting
{
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

    internal class GeneratePcsXmlHandler : IRequestHandler<GeneratePcsXml, GeneratedXmlFile>
    {
        public Task<GeneratedXmlFile> HandleAsync(GeneratePcsXml message)
        {
            ProducerList producerList = ProducerList.Create(message.Settings);

            XmlGeneratorFactory xmlGeneratorFactory = new XmlGeneratorFactory();

            IXmlGenerator xmlGenerator = xmlGeneratorFactory.CreateGenerator(message.Settings.SchemaVersion);

            XDocument xml = xmlGenerator.GenerateXml(producerList);

            string fileName = string.Format("PCS Member Upload XML - {0:yyyy MM dd HH mm ss}.xml", DateTime.UtcNow);

            byte[] data;
            
            using (MemoryStream stream = new MemoryStream())
            {
                xml.Save(stream);
                data = stream.ToArray();
            }

            GeneratedXmlFile file = new GeneratedXmlFile(fileName, data);

            return Task.FromResult(file);
        }
    }
}
