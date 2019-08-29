namespace EA.Prsd.Core.Mediator
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class MissingHandlerException : Exception
    {
        public MissingHandlerException()
        {
        }

        public MissingHandlerException(string message) : base(message)
        {
        }

        public MissingHandlerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MissingHandlerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
