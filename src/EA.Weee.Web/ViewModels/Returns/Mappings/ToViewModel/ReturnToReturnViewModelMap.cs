namespace EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Shared;

    public class ReturnToReturnViewModelMap : IMap<ReturnData, ReturnViewModel>
    {
        public List<AatfObligatedData> AatfObligatedData = new List<AatfObligatedData>();
        public TaskListDisplayOptions DisplayOptions = new TaskListDisplayOptions();
        private readonly ITonnageUtilities tonnageUtilities;

        public ReturnToReturnViewModelMap(ITonnageUtilities tonnageUtilities)
        {
            this.tonnageUtilities = tonnageUtilities;
        }

        public ReturnViewModel Map(ReturnData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            SetObligatedValues(source);

            SetDisplayOptions(source);

            decimal? totalNonObligatedTonnageDcf = null;
            decimal? totalNonObligatedTonnage = null;

            if (source.NonObligatedData != null)
            {
                if (source.NonObligatedData.Any(n => n.Dcf && n.Tonnage.HasValue))
                {
                    totalNonObligatedTonnageDcf = tonnageUtilities.InitialiseTotalDecimal(source.NonObligatedData.Where(n => n.Dcf && n.Tonnage.HasValue).Sum(n => n.Tonnage));
                }

                if (source.NonObligatedData.Any(n => !n.Dcf && n.Tonnage.HasValue))
                {
                    totalNonObligatedTonnage = tonnageUtilities.InitialiseTotalDecimal(source.NonObligatedData.Where(n => !n.Dcf && n.Tonnage.HasValue).Sum(n => n.Tonnage));
                }
            }

            return new ReturnViewModel(
                source,
                AatfObligatedData,
                source.OrganisationData,
                DisplayOptions)
            {
                NonObligatedTonnageTotal = tonnageUtilities.CheckIfTonnageIsNull(totalNonObligatedTonnage),
                NonObligatedTonnageTotalDcf = tonnageUtilities.CheckIfTonnageIsNull(totalNonObligatedTonnageDcf),
                NonObligatedTotal = tonnageUtilities.CheckIfTonnageIsNull(totalNonObligatedTonnage),
                ObligatedTotal = tonnageUtilities.CheckIfTonnageIsNull(TotalObligated(source))
            };
        }

        private void SetObligatedValues(ReturnData source)
        {
            if (source.Aatfs != null)
            {
                foreach (var aatf in source.Aatfs)
                {
                    var weeeReceivedData = source.ObligatedWeeeReceivedData.Where(s => s.Aatf.Id == aatf.Id).ToList();
                    var weeeReusedData = source.ObligatedWeeeReusedData.Where(s => s.Aatf.Id == aatf.Id).ToList();
                    var weeeSentOnData = source.ObligatedWeeeSentOnData.Where(s => s.Aatf.Id == aatf.Id).ToList();

                    var schemeData = new List<AatfSchemeData>();

                    foreach (var scheme in source.SchemeDataItems)
                    {
                        var schemeList = weeeReceivedData.Where(s => s.Scheme.Id == scheme.Id && s.Aatf.Id == aatf.Id).ToList();

                        var obligatedReceivedValues = new ObligatedCategoryValue
                        {
                            B2B = tonnageUtilities.SumObligatedValues(schemeList).B2B,
                            B2C = tonnageUtilities.SumObligatedValues(schemeList).B2C
                        };

                        var aatfSchemeData = new AatfSchemeData(scheme, obligatedReceivedValues, scheme.ApprovalName);
                        schemeData.Add(aatfSchemeData);
                    }

                    var obligatedData = new AatfObligatedData(aatf, schemeData)
                    {
                        WeeeReceived = tonnageUtilities.SumObligatedValues(weeeReceivedData),
                        WeeeReused = tonnageUtilities.SumObligatedValues(weeeReusedData),
                        WeeeSentOn = tonnageUtilities.SumObligatedValues(weeeSentOnData)
                    };

                    AatfObligatedData.Add(obligatedData);
                }
            }
        }

        private void SetDisplayOptions(ReturnData source)
        {
            if (source.ReturnReportOns != null)
            {
                foreach (var option in source.ReturnReportOns)
                {
                    switch (option.ReportOnQuestionId)
                    {
                        case 1:
                            DisplayOptions.DisplayObligatedReceived = true;
                            break;
                        case 2:
                            DisplayOptions.DisplayObligatedSentOn = true;
                            break;
                        case 3:
                            DisplayOptions.DisplayObligatedReused = true;
                            break;
                        case 4:
                            DisplayOptions.DisplayNonObligated = true;
                            break;
                        case 5:
                            DisplayOptions.DisplayNonObligatedDcf = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private decimal? TotalObligated(ReturnData returnData)
        {
            decimal? total = null;

            if (returnData.ObligatedWeeeReceivedData.Any(o => o.Total.HasValue))
            {
                total = tonnageUtilities.InitialiseTotalDecimal(returnData.ObligatedWeeeReceivedData.Sum(o => o.Total));
            }

            return total;
        }
    }
}