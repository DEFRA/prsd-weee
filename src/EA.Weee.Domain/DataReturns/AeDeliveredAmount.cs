namespace EA.Weee.Domain.DataReturns
{
    using System.Diagnostics.CodeAnalysis;
    using Lookup;
    using Prsd.Core;

    public class AeDeliveredAmount : WeeeDeliveredAmount
    {
        public virtual AeDeliveryLocation AeDeliveryLocation { get; private set; }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "aeDeliveryLocation name is valid.")]
        public AeDeliveredAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, AeDeliveryLocation aeDeliveryLocation) :
            base(obligationType, weeeCategory, tonnage)
        {
            Guard.ArgumentNotNull(() => aeDeliveryLocation, aeDeliveryLocation);

            AeDeliveryLocation = aeDeliveryLocation;
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected AeDeliveredAmount()
        {
        }
    }
}
