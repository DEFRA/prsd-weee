namespace EA.Weee.Domain.AatfReturn
{
    using Prsd.Core;
    using Prsd.Core.Domain;
    using System;

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
