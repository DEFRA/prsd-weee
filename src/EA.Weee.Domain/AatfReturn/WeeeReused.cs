namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using Scheme;

    public class WeeeReused : Entity
    {
        public Guid SchemeId { get; private set; }

        public Guid AatfId { get; private set; }

        public virtual Aatf Aatf { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public Guid ReturnId { get; private set; }

        public WeeeReused(Guid schemeId, Guid aatfId, Guid returnId)
        {
            SchemeId = schemeId;
            AatfId = aatfId;
            ReturnId = returnId;
        }

        public WeeeReused(Scheme scheme, Aatf aatf, Guid returnId)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);
            Guard.ArgumentNotNull(() => aatf, aatf);

            Scheme = scheme;
            Aatf = aatf;
            ReturnId = returnId;
        }

        public WeeeReused()
        {
        }
    }
}
