namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using System;
    using Core.AatfReturn;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;
    using Prsd.Core;
    using WebGrease.Css.Extensions;

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

            var groupedEdits = model.Returns.Where(r => r.ReturnsListDisplayOptions.DisplayEdit)
                .OrderByDescending(r => Convert.ToDateTime(r.ReturnViewModel.CreatedDate))
                .GroupBy(r => new {r.ReturnViewModel.Year, r.ReturnViewModel.Quarter});

            foreach (var groupedEdit in groupedEdits)
            {
                var quarter = groupedEdit.Key.Quarter;
                var year = groupedEdit.Key.Year;

                if (model.Returns.Any(r =>
                    r.ReturnViewModel.Quarter == quarter &&
                    r.ReturnViewModel.Year == year &&
                    r.ReturnsListDisplayOptions.DisplayContinue))
                {
                    groupedEdit.ForEach(r => r.ReturnsListDisplayOptions.DisplayEdit = false);
                }
                else
                {
                    groupedEdit.Skip(1).ForEach(r => r.ReturnsListDisplayOptions.DisplayEdit = false);
                }
            }

            if (source.ReturnQuarter != null)
            {
                model.DisplayCreateReturn = true;
                model.ComplianceYear = source.ReturnQuarter.Year;
                model.Quarter = source.ReturnQuarter.Q;
            }

            return model;
        }
    }
}