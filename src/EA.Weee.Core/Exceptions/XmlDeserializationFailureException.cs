namespace EA.Weee.Core.Exceptions
{
    using System;

    public class XmlDeserializationFailureException : Exception
    {
        private const string XmlDeserializationFailureMessage = "Unable to deserialize XML. See inner exception for details.";

        public XmlDeserializationFailureException(Exception innerException)
            : base(XmlDeserializationFailureMessage, innerException)
        {
        }
    }
}