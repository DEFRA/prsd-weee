namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;
    using Prsd.Core;

    public class ReturnsToReturnsViewModelMap : IMap<ReturnsData, ReturnsViewModel>
    {
        private readonly IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap;
        private readonly IReturnsOrdering ordering;

        public ReturnsToReturnsViewModelMap(IReturnsOrdering ordering, IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap)
        {
            this.ordering = ordering;
            this.returnItemViewModelMap = returnItemViewModelMap;
        }

        public ReturnsViewModel Map(ReturnsData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ReturnsViewModel();

            var orderedItems = ordering.Order(source.ReturnsList);
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

            if (source.ReturnQuarter != null)
            {
                model.DisplayCreateReturn = true;
                model.ComplianceYear = source.ReturnQuarter.ComplianceYear;
                model.Quarter = source.ReturnQuarter.Quarter;
            }

            return model;
        }
    }
}