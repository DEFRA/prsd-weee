namespace EA.Weee.Domain.AatfReturn
{
    using Domain.Scheme;
    using EA.Prsd.Core;
    using System;

    public partial class ReturnScheme : ReturnEntity, IReturnOption
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
            this.ReturnId = @return.Id;
            this.SchemeId = scheme.Id;
        }

        public virtual Scheme Scheme { get; private set; }

        public virtual Guid SchemeId { get; private set; }
    }
}
