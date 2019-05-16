namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnsToReturnsViewModelMap : IMap<List<ReturnData>, ReturnsViewModel>
    {
        private readonly IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap;
        private readonly IReturnsOrdering ordering;

        public ReturnsToReturnsViewModelMap(IReturnsOrdering ordering, IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap)
        {
            this.ordering = ordering;
            this.returnItemViewModelMap = returnItemViewModelMap;
        }

        public ReturnsViewModel Map(List<ReturnData> source)
        {
            var model = new ReturnsViewModel();

            var orderedItems = ordering.Order(source);
            foreach (var @return in orderedItems)
            {
                var returnViewModelItem = returnItemViewModelMap.Map(@return);

                // get all created, find if any of the return view model 
                var inProgress = source.Where(r => r.ReturnStatus == ReturnStatus.Created);

                model.Returns.Add(returnViewModelItem);
            }

            return model;
        }
    }
}