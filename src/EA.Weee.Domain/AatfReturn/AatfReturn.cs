namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core.Domain;

    public class AatfReturn : Entity
    {
        public Guid ReturnId { get; private set; }

        public Guid AatfId { get; private set; }

        public AatfReturn(Guid returnId, Guid aatfId)
        {
            ReturnId = returnId;
            AatfId = aatfId;
        }
    }
}
