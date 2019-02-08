namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core.Domain;

    public class AatfWeeReceived : Entity
    {
        public Guid SchemeId { get; private set; }

        public Guid ReturnId { get; private set; }

        public AatfWeeReceived(Guid returnId, Guid schemeId)
        {
            SchemeId = schemeId;
            ReturnId = returnId;
        }
    }
}
