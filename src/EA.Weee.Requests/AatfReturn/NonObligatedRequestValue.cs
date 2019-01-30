namespace EA.Weee.Requests.AatfReturn
{
    using Core.DataReturns;
    using Core.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NonObligatedRequestValue
    {
        public int CategoryId { get; private set; }

        public decimal? Tonnage { get; private set; }

        public bool Dcf { get; private set; }

        public NonObligatedRequestValue(int categoryId, decimal? tonnage, bool dcf)
        {
            CategoryId = categoryId;
            Tonnage = tonnage;
            Dcf = dcf;
        }
    }
}
