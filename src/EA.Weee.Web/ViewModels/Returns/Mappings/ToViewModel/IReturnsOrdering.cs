namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using Core.AatfReturn;

    public interface IReturnsOrdering
    {
        IEnumerable<ReturnData> Order(List<ReturnData> data);
    }
}