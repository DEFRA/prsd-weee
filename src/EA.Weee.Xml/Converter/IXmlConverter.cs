﻿namespace EA.Weee.Xml.Converter
{
    using MemberRegistration;
    using System.Xml.Linq;

    public interface IXmlConverter
    {
        XDocument Convert(byte[] data);

        string XmlToUtf8String(byte[] data);

        schemeType Deserialize(XDocument xdoc);
    }
}
