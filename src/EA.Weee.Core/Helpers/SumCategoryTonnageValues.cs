namespace EA.Weee.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfReturn;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;

    public class SumCategoryTonnageValues : ISumCategoryTonnageValues
    {
        public string SumTonnageValues<T>(IList<T> categoryValues) where T : CategoryValue
        {
            throw new NotImplementedException();
        }
    }
}
