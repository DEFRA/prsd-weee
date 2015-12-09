namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;

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
            AmountInTonnes = amountInTonnes;
        }
    }
}
