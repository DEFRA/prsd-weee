namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnsToReturnsViewModelMap : IMap<List<ReturnData>, ReturnsViewModel>
    {
        private readonly IMap<ReturnData, ReturnViewModel> returnViewModelmap;
        private readonly IMap<ReturnViewModel, ReturnsItemViewModel> returnItemViewModelmap;
        private readonly IReturnsOrdering ordering;

        public ReturnsToReturnsViewModelMap(IMap<ReturnData, ReturnViewModel> returnViewModelmap, 
            IReturnsOrdering ordering, IMap<ReturnViewModel, ReturnsItemViewModel> returnItemViewModelmap)
        {
            this.returnViewModelmap = returnViewModelmap;
            this.ordering = ordering;
            this.returnItemViewModelmap = returnItemViewModelmap;
        }

        public ReturnsViewModel Map(List<ReturnData> source)
        {
            var model = new ReturnsViewModel();

            var orderedItems = ordering.Order(source);
            foreach (var @return in orderedItems)
            {
                var returnViewModelItem = returnItemViewModelmap.Map(returnViewModelmap.Map(@return));

                model.Returns.Add(returnViewModelItem);
            }

            return model;
        }
    }
}