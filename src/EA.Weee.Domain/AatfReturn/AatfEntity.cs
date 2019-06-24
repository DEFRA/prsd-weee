namespace EA.Weee.Domain.AatfReturn
{
    using System;

    public class AatfEntity : ReturnEntity
    {
        public virtual Guid AatfId { get; protected set; }

        public virtual Aatf Aatf { get; protected set; }
    }
}
