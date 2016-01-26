namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Lookup;
    using Obligation;
    using Prsd.Core;

    public class WeeeDeliveredAmount : ReturnItem, IEquatable<WeeeDeliveredAmount>
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

        public bool Equals(WeeeDeliveredAmount other)
        {
            return Equals((ReturnItem)other) &&
                   Equals(AatfDeliveryLocation, other.AatfDeliveryLocation) &&
                   Equals(AeDeliveryLocation, other.AeDeliveryLocation);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WeeeDeliveredAmount);
        }

        public override int GetHashCode()
        {
            int baseHash = base.GetHashCode();

            return IsAatfDeliveredAmount ? baseHash ^ AatfDeliveryLocation.GetHashCode() : baseHash ^ AeDeliveryLocation.GetHashCode();
        }
    }
}
