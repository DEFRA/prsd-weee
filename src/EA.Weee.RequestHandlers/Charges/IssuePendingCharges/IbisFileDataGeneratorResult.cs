namespace EA.Weee.RequestHandlers.Charges.IssuePendingCharges
{
    using Domain.Charges;
    using System.Collections.Generic;

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
