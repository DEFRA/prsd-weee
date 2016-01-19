namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using Ibis;

    public class IbisFileGeneratorResult<T> where T : IbisFile
    {
        public T IbisFile { get; private set; }

        public List<Exception> Errors { get; private set; }

        public IbisFileGeneratorResult(T ibisFile, List<Exception> errors)
        {
            IbisFile = ibisFile;
            Errors = errors;
        }
    }
}
