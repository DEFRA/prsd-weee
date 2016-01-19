namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using System;
    using System.Collections.Generic;
    using Domain.Charges;

    public class IbisFileDataGeneratorResult
    {
        public IbisFileData IbisFileData { get; private set; }

        public List<string> Errors { get; private set; }

        public IbisFileDataGeneratorResult(IbisFileData ibisFileData, List<string> errors)
        {
            IbisFileData = ibisFileData;
            Errors = errors;
        }
    }
}
