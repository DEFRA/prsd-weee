namespace EA.Weee.Domain.DataReturns
{
    using System.Collections.Generic;
    using Lookup;
    using Prsd.Core;

    public abstract class WeeeDeliveredAmount : ReturnItem
    {
        public virtual ICollection<WeeeDeliveredReturnVersion> WeeeDeliveredReturnVersions { get; private set; }

        public WeeeDeliveredAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage) :
            base(obligationType, weeeCategory, tonnage)
        {
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected WeeeDeliveredAmount()
        {
        }
    }
}
