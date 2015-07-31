namespace EA.Weee.RequestHandlers.PCS.MemberUploadTesting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
