namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;

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
                var returnViewModelItems = returnItemViewModelMap.Map(@return);

                model.Returns.Add(returnViewModelItems);
            }

            foreach (var returnsItemViewModel in model.Returns.Where(r => r.ReturnsListDisplayOptions.DisplayEdit))
            {
                if (model.Returns.Any(r =>
                    r.ReturnViewModel.Quarter == returnsItemViewModel.ReturnViewModel.Quarter &&
                    r.ReturnViewModel.Year == returnsItemViewModel.ReturnViewModel.Year &&
                    r.ReturnsListDisplayOptions.DisplayContinue))
                {
                    returnsItemViewModel.ReturnsListDisplayOptions.DisplayEdit = false;
                }
            }

            return model;
        }
    }
}