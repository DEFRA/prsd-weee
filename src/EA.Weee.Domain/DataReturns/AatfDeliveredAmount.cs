namespace EA.Weee.Domain.DataReturns
{
    using Lookup;
    using Prsd.Core;

    public class AatfDeliveredAmount : WeeeDeliveredAmount
    {
        public virtual AatfDeliveryLocation AatfDeliveryLocation { get; private set; }

        public AatfDeliveredAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, AatfDeliveryLocation aatfDeliveryLocation) :
            base(obligationType, weeeCategory, tonnage)
        {
            Guard.ArgumentNotNull(() => aatfDeliveryLocation, aatfDeliveryLocation);

            AatfDeliveryLocation = aatfDeliveryLocation;
        }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected AatfDeliveredAmount()
        {
        }
    }
}
