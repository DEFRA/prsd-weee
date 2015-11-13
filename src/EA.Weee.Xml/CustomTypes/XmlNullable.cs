namespace EA.Weee.Xml.CustomTypes
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    public class XmlNullable<T> : IXmlSerializable
    {
        public T Value { get; private set; }

        public bool HasValue { get; private set; }

        public XmlNullable()
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                throw new InvalidOperationException("Type must be a non-nullable type");
            }

            HasValue = false;
        }

        public XmlNullable(T value)
        {
            Value = value;
            HasValue = true;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmpty)
            {
                var str = reader.ReadString();
                if (!string.IsNullOrEmpty(str))
                {
                    Value = (T)Convert.ChangeType(str, typeof(T));
                    HasValue = true;
                }

                reader.ReadEndElement();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException("This type should only be used in deserialization");
        }
    }
}
