namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class WeeeReused : Entity
    {
        public virtual Guid AatfId { get; private set; }

        public virtual Aatf Aatf { get; private set; }
        
        public virtual Guid ReturnId { get; private set; }

        public WeeeReused(Guid aatfId, Guid returnId)
        {
            AatfId = aatfId;
            ReturnId = returnId;
        }

        public WeeeReused(Aatf aatf, Guid returnId)
        {
            Guard.ArgumentNotNull(() => aatf, aatf);

            Aatf = aatf;
            ReturnId = returnId;
        }

        public WeeeReused()
        {
        }
    }
}
