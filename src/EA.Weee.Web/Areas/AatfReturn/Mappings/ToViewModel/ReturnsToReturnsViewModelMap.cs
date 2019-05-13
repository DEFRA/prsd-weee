namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnsToReturnsViewModelMap : IMap<List<ReturnData>, ReturnsViewModel>
    {
        private readonly IMap<ReturnData, ReturnViewModel> returnViewModelMap;
        private readonly IMap<ReturnViewModel, ReturnsItemViewModel> returnItemViewModelMap;
        private readonly IReturnsOrdering ordering;

        public ReturnsToReturnsViewModelMap(IMap<ReturnData, ReturnViewModel> returnViewModelMap, 
            IReturnsOrdering ordering, IMap<ReturnViewModel, ReturnsItemViewModel> returnItemViewModelMap)
        {
            this.returnViewModelMap = returnViewModelMap;
            this.ordering = ordering;
            this.returnItemViewModelMap = returnItemViewModelMap;
        }

        public ReturnsViewModel Map(List<ReturnData> source)
        {
            var model = new ReturnsViewModel();

            var orderedItems = ordering.Order(source);
            foreach (var @return in orderedItems)
            {
                var returnViewModelItem = returnItemViewModelMap.Map(returnViewModelMap.Map(@return));

                model.Returns.Add(returnViewModelItem);
            }

            return model;
        }
    }
}