namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.AatfReturn;

    public class ReturnsOrdering : IReturnsOrdering
    {
        public IEnumerable<ReturnData> Order(List<ReturnData> data)
        {
            return data.OrderByDescending(r => r.Quarter.Year)
                .ThenByDescending(r => (int)r.Quarter.Q)
                .ThenByDescending(r => r.CreatedDate);
        }
    }
}