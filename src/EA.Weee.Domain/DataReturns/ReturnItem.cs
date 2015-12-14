namespace EA.Weee.Domain.DataReturns
{
    using System;
    using Lookup;
    using Prsd.Core.Domain;

    public class ReturnItem : Entity, IReturnItem
    {
        public virtual ObligationType ObligationType { get; private set; }

        public virtual decimal Tonnage { get; private set; }

        public virtual WeeeCategory WeeeCategory { get; private set; }

        public ReturnItem(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage)
        {
            WeeeCategory = weeeCategory;
            ObligationType = obligationType;

            if (tonnage < 0)
            {
                throw new ArgumentOutOfRangeException("amountInTonnes");
            }

            if (obligationType != ObligationType.B2B && obligationType != ObligationType.B2C)
            {
                string errorMessage = "The obligation type of a return item must be either B2B or B2C.";
                throw new InvalidOperationException(errorMessage);
            }

            Tonnage = tonnage;
        }

        protected ReturnItem()
        {
        }
    }
}
