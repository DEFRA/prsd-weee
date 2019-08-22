namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using System.Collections.Generic;

    public interface IReturnsOrdering
    {
        IEnumerable<ReturnData> Order(List<ReturnData> data);
    }
}