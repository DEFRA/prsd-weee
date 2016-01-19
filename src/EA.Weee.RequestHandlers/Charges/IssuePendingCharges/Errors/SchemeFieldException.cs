namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges.Errors
{
    using System;
    using Scheme = Domain.Scheme.Scheme;

    public class SchemeFieldException : Exception
    {
        public Scheme Scheme { get; private set; }

        public Exception Exception { get; private set; }

        public SchemeFieldException(Scheme scheme, Exception exception)
        {
            Scheme = scheme;
            Exception = exception;
        }
    }
}
