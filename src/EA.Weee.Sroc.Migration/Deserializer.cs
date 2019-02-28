﻿namespace EA.Weee.Sroc.Migration
{
    using EA.Weee.Xml.Deserialization;
    using System;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public class Deserializer : IDeserializer
    {
        public T Deserialize<T>(XDocument document)
        {
            try
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(document.CreateReader());
            }
            catch (InvalidOperationException ex)
            {
                throw new XmlDeserializationFailureException(ex);
            }
        }
    }
}
