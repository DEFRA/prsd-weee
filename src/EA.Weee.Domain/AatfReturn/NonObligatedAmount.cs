namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NonObligatedAmount
    {
        public decimal? Tonnage { get; set; }

        public virtual void UpdateTonnage(decimal? tonnage)
        {
            Tonnage = tonnage;
        }
    }
}
