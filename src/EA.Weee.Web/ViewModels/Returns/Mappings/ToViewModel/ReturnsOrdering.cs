namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using System.Collections.Generic;
    using System.Linq;

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