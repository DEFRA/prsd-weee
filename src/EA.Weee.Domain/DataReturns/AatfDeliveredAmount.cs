namespace EA.Weee.Domain.DataReturns
{
    using Lookup;
    using Prsd.Core;

    public class AatfDeliveredAmount : WeeeDeliveredAmount
    {
        public virtual AatfDeliveryLocation AatfDeliveryLocation { get; private set; }

        public AatfDeliveredAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, AatfDeliveryLocation aatfDeliveryLocation, DataReturnVersion dataReturnVersion) :
            base(obligationType, weeeCategory, tonnage, dataReturnVersion)
        {
            Guard.ArgumentNotNull(() => aatfDeliveryLocation, aatfDeliveryLocation);

            AatfDeliveryLocation = aatfDeliveryLocation;
        }

        protected AatfDeliveredAmount()
        {
        }
    }
}
