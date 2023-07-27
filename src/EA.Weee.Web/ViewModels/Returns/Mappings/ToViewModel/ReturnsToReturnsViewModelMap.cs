namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Helpers;
    using Microsoft.Owin;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using WebGrease.Css.Extensions;
    using Weee.Requests.Organisations;

    public class ReturnsToReturnsViewModelMap : IMap<ReturnToReturnsViewModelTransfer, ReturnsViewModel>
    {
        private readonly IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap;
        private readonly IReturnsOrdering ordering;

        private const string NotExpectedError = "You aren’t expected to submit a return yet. If you think this is wrong, contact your environmental regulator.";

        public ReturnsToReturnsViewModelMap(IReturnsOrdering ordering, IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap)
        {
            this.ordering = ordering;
            this.returnItemViewModelMap = returnItemViewModelMap;
        }

        public ReturnsViewModel Map(ReturnToReturnsViewModelTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.ReturnsData, source.ReturnsData);

            var model = new ReturnsViewModel { NumberOfReturns = source.ReturnsData.ReturnsList.Count() };

            OrderReturns(source.ReturnsData, model);

            SetEditableReturns(model);

            SetMessages(source.ReturnsData, model);

            FilterReturnsBasedOnParameters(source, model);

            return model;
        }

        private static void FilterReturnsBasedOnParameters(ReturnToReturnsViewModelTransfer source, ReturnsViewModel model)
        {
            if (source.ReturnsData.ReturnsList.Any(x => x.Quarter != null))
            {
                var complianceYearList = source.ReturnsData.ReturnsList.Select(x => x.Quarter.Year).OrderByDescending(x => x).Distinct().ToList();
                model.ComplianceYearList = complianceYearList;

                if (source.SelectedComplianceYear.HasValue)
                {
                    model.SelectedComplianceYear = source.SelectedComplianceYear.Value;
                    model.QuarterList = source.ReturnsData.ReturnsList.Where(x => x.Quarter.Year == source.SelectedComplianceYear.Value).Select(x => x.Quarter.Q.ToString()).Distinct().ToList();
                }
                else
                {
                    var latestComplianceYear = complianceYearList.OrderByDescending(x => x).FirstOrDefault();
                    model.QuarterList = source.ReturnsData.ReturnsList.Where(x => x.Quarter.Year == latestComplianceYear).Select(x => x.Quarter.Q.ToString()).Distinct().ToList();
                    model.SelectedComplianceYear = latestComplianceYear;
                }

                model.Returns = model.Returns.Where(p => p.ReturnViewModel.Year == model.SelectedComplianceYear.ToString()).ToList();

                if (source.SelectedQuarter != null)
                {
                    model.SelectedQuarter = source.SelectedQuarter;

                    if (model.SelectedQuarter != "All")
                    {
                        model.Returns = model.Returns.Where(p => p.ReturnViewModel.Quarter == model.SelectedQuarter).ToList();
                    }
                }
                else
                {
                    model.SelectedQuarter = "All";
                }
            }
        }

        private void OrderReturns(ReturnsData source, ReturnsViewModel model)
        {
            var orderedItems = ordering.Order(source.ReturnsList);
            foreach (var @return in orderedItems)
            {
                var returnViewModelItems = returnItemViewModelMap.Map(@return);

                model.Returns.Add(returnViewModelItems);
            }
        }

        private static void SetEditableReturns(ReturnsViewModel model)
        {
            var groupedEdits = model.Returns.Where(r => r.ReturnsListDisplayOptions.DisplayEdit)
                .OrderByDescending(r => DateTime.ParseExact(r.ReturnViewModel.CreatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))
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
                    groupedEdit.ForEach<ReturnsItemViewModel>(r => r.ReturnsListDisplayOptions.DisplayEdit = false);
                }
                else
                {
                    groupedEdit.Skip<ReturnsItemViewModel>(1).ForEach(r => r.ReturnsListDisplayOptions.DisplayEdit = false);
                }
            }
        }

        private void SetMessages(ReturnsData source, ReturnsViewModel model)
        {
            if (source.ReturnQuarter != null)
            {
                model.DisplayCreateReturn = true;
                model.ComplianceYear = source.ReturnQuarter.Year;
                model.Quarter = source.ReturnQuarter.Q;
            }
            else
            {
                model.ErrorMessageForNotAllowingCreateReturn = WorkOutErrorMessageForNotAllowingCreateReturn(source);

                model.NotStartedAnySubmissionsYet = model.ErrorMessageForNotAllowingCreateReturn != NotExpectedError;
            }
        }

        private string WorkOutErrorMessageForNotAllowingCreateReturn(ReturnsData source)
        {
            if (!WindowHelper.IsThereAnOpenWindow(source.CurrentDate))
            {
                return
                    $"The {source.CurrentDate.AddYears(-1).Year} compliance period has closed. You can start submitting your {source.CurrentDate.Year} Q1 returns on 1st April.";
            }
            foreach (Quarter q in source.OpenQuarters)
            {
                if (source.ReturnsList.Count(p => p.Quarter == q) > 0)
                {
                    QuarterType nextQuarter = WorkOutNextQuarter(source.OpenQuarters);

                    int startYear;
                    if (nextQuarter == QuarterType.Q4)
                    {
                        startYear = source.NextWindow.WindowOpenDate.Year - 1;
                    }
                    else
                    {
                        startYear = source.NextWindow.WindowOpenDate.Year;
                    }

                    return
                        $"Returns have been started or submitted for all open quarters. You can start submitting your {startYear} {nextQuarter} returns on {source.NextWindow.WindowOpenDate.ToReadableDateTime()}.";
                }
            }
            return NotExpectedError;
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