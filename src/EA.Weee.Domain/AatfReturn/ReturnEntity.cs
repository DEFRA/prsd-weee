namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class ReturnEntity : Entity
    {
        public virtual Return Return { get; protected set; }

        public virtual Guid ReturnId { get; protected set; }

        public void UpdateReturn(Return @return)
        {
            Guard.ArgumentNotNull(() => @return, @return);

            Return = @return;
        }
    }
}
