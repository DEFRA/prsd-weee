namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using Lookup;
    public class ReturnItem : Entity
    {
        public Category Category { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public decimal AmountInTonnes { get; private set; }

        public ReturnItem(
            Category category,
            ObligationType obligationType,
            decimal amountInTonnes)
        {
            Category = category;
            ObligationType = obligationType;

            if (amountInTonnes < 0)
            {
                throw new ArgumentOutOfRangeException("amountInTonnes");
            }

            if (obligationType != ObligationType.B2B && obligationType != ObligationType.B2C)
            {
                string errorMessage = "The obligation type of a return item must be either B2B or B2C.";
                throw new InvalidOperationException(errorMessage);
            }

            AmountInTonnes = amountInTonnes;
        }
    }
}
