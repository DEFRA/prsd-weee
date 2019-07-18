namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                .GroupBy(r => new { r.ReturnViewModel.Year, r.ReturnViewModel.Quarter });

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
            else
            {
                model.ErrorMessageForNotAllowingCreateReturn = WorkOutErrorMessageForNotAllowingCreateReturn(source);
            }

            return model;
        }

        private string WorkOutErrorMessageForNotAllowingCreateReturn(ReturnsData source)
        {
            if (!WindowHelper.IsThereAnOpenWindow())
            {
                return string.Format("The {0} compliance period has closed. You can start submitting your {1} Q1 returns on 1st April.", SystemTime.Now.AddYears(-1).Year, SystemTime.Now.Year);
            }
            foreach (Quarter q in source.OpenQuarters)
            {
                if (source.ReturnsList.Count(p => p.Quarter == q) > 0)
                {
                    QuarterType nextQuarter = WorkOutNextQuarter(source.OpenQuarters);

                    return string.Format("Returns have been started or submitted for all open quarters. You can start submitting your {0} {1} returns on {2}.", SystemTime.Now.Year, nextQuarter, source.NextWindow.WindowOpenDate.ToShortDateString());
                }
            }
            return "You aren’t expected to submit a return yet. If you think this is wrong, contact your environmental regulator.";
        }

        private QuarterType WorkOutNextQuarter(List<Quarter> openQuarters)
        {
            QuarterType latestOpen = openQuarters.OrderByDescending(p => p.Q).First().Q;

            if ((int)latestOpen == 4)
            {
                return QuarterType.Q1;
            }

            return (QuarterType)((int)latestOpen + 1);
        }
    }
}