namespace EA.Weee.RequestHandlers.Scheme.MemberUploadTesting
{
    using System.IO;
    using System.Xml;

    /// <summary>
    /// This implementation of XmlTextWriter will corrupt the starting tag of a specified element
    /// by writing it within an additional pair of angle brackets.
    /// </summary>
    internal class BrokenXmlWriter : XmlTextWriter
    {
        public string BrokenElementName { get; private set; }
        
        public BrokenXmlWriter(TextWriter textWriter, string brokenElementName)
            : base(textWriter)
        {
            BrokenElementName = brokenElementName;
            Formatting = Formatting.Indented;
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            if (localName == BrokenElementName)
            {
                WriteRaw("<");
            }

            base.WriteStartElement(prefix, localName, ns);

            if (localName == BrokenElementName)
            {
                WriteRaw(">");
            }
        }
    }
}
