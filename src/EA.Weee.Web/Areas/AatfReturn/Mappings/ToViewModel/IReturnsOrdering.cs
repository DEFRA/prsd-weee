namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.AatfReturn;

    public interface IReturnsOrdering
    {
        IEnumerable<ReturnData> Order(List<ReturnData> data);
    }
}