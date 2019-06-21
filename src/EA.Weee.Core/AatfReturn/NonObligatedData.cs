namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NonObligatedData
    {
        public NonObligatedData(int categoryId, decimal? tonnage, bool dcf, Guid id)
        {
            CategoryId = categoryId;
            Tonnage = tonnage;
            Dcf = dcf;
            Id = id;
        }

        public int CategoryId { get; private set; }

        public decimal? Tonnage { get; private set; }

        public bool Dcf { get; private set; }

        public Guid Id { get; private set; }
    }
}
