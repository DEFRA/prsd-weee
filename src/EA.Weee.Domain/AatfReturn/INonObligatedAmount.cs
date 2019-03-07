namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface INonObligatedAmount
    {
        decimal? Tonnage { get; set; }
    }
}
