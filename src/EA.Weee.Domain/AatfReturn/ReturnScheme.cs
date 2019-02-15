namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using DataReturns;
    using Domain.Scheme;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class ReturnScheme : Entity
    {
        protected ReturnScheme()
        {
        }

        public ReturnScheme(Scheme scheme, Return @return)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);
            Guard.ArgumentNotNull(() => @return, @return);

            this.Scheme = scheme;
            this.Return = @return;
            this.ReturnId = Return.Id;
            this.SchemeId = Scheme.Id;
        }

        public virtual Scheme Scheme { get; private set; }

        public virtual Return Return { get; private set; }

        public Guid ReturnId { get; private set; }

        public Guid SchemeId { get; private set; }
    }
}
