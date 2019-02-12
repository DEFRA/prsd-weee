namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using DataReturns;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class ReturnScheme : Entity
    {
        protected ReturnScheme()
        {
        }

        public ReturnScheme(Guid schemeId, Guid returnId)
        {
            this.SchemeId = schemeId;
            this.ReturnId = returnId;
        }

        public virtual Guid SchemeId { get; private set; }

        public virtual Guid ReturnId { get; private set; }
    }
}
