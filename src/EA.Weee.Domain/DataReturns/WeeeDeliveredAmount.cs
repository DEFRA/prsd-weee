namespace EA.Weee.Domain.DataReturns
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Lookup;
    using Prsd.Core;

    public class WeeeDeliveredAmount : ReturnItem
    {
        public virtual AatfDeliveryLocation AatfDeliveryLocation { get; private set; }

        public virtual AeDeliveryLocation AeDeliveryLocation { get; private set; }

        public virtual ICollection<WeeeDeliveredReturnVersion> WeeeDeliveredReturnVersions { get; private set; }

        public WeeeDeliveredAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, AatfDeliveryLocation aatfDeliveryLocation) :
            base(obligationType, weeeCategory, tonnage)
        {
            Guard.ArgumentNotNull(() => aatfDeliveryLocation, aatfDeliveryLocation);
            AatfDeliveryLocation = aatfDeliveryLocation;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Parameter name aeDeliveryLocation is valid.")]
        public WeeeDeliveredAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, AeDeliveryLocation aeDeliveryLocation) :
            base(obligationType, weeeCategory, tonnage)
        {
            Guard.ArgumentNotNull(() => aeDeliveryLocation, aeDeliveryLocation);

            AeDeliveryLocation = aeDeliveryLocation;
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected WeeeDeliveredAmount()
        {
        }

        public bool IsAatfDeliveredAmount
        {
            get { return AatfDeliveryLocation != null; }
        }

        public bool IsAeDeliveredAmount
        {
            get { return AeDeliveryLocation != null; }
        }
    }
}
