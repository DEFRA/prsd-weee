namespace EA.Weee.Tests.Core.Xml
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;

    public class XmlSchemaHelper
    {
        private static Dictionary<string, XmlSchemaSet> cachedXmlSchemas = new Dictionary<string, XmlSchemaSet>();

        public string SchemaFile { get; private set; }

        private XmlSchemaSet schemaSet;

        public XmlSchemaHelper(string schemaFile)
        {
            SchemaFile = schemaFile;
        }

        private void LoadSchema()
        {
            if (schemaSet == null &&
                !cachedXmlSchemas.TryGetValue(SchemaFile, out schemaSet))
            {
                XmlSchema schema;
                using (var schemaFileStream = File.Open(SchemaFile, FileMode.Open))
                {
                    schema = XmlSchema.Read(schemaFileStream, null);
                }

                schemaSet = new XmlSchemaSet();
                schemaSet.Add(schema);

                cachedXmlSchemas.Add(SchemaFile, schemaSet);
            }
        }

        public ValidationEventArgs ValidateXmlWithSingleResult(string filename)
        {
            var result = ValidateXml(filename);

            return result.Single();
        }

        public List<ValidationEventArgs> ValidateXml(string filename)
        {
            using (var reader = XmlReader.Create(filename))
            {
                var xdoc = XDocument.Load(reader, LoadOptions.SetLineInfo);
                return ValidateXml(xdoc);
            }
        }

        public List<ValidationEventArgs> ValidateXml(XDocument document)
        {
            LoadSchema();

            var validationEventArgs = new List<ValidationEventArgs>();

            document.Validate(schemaSet,
                    (o, args) =>
                    {
                        validationEventArgs.Add(args);
                    });

            return validationEventArgs;
        }
    }
}
