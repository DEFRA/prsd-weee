namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core.Domain;

    public class WeeeReceived : Entity
    {
        public Guid SchemeId { get; private set; }

        public Guid AatfId { get; private set; }

        public Guid ReturnId { get; private set; }

        public WeeeReceived(Guid schemeId, Guid aatfId, Guid returnId)
        {
            SchemeId = schemeId;
            AatfId = aatfId;
            ReturnId = returnId;
        }

        public WeeeReceived()
        {
        }
    }
}
