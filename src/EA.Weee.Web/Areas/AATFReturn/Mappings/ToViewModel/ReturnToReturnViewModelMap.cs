namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnToReturnViewModelMap : IMap<ReturnData, ReturnViewModel>
    {
        public decimal? NonObligatedTonnageTotal = 0.000m;
        public decimal? NonObligatedTonnageTotalDcf = 0.000m;
        public decimal? ObligatedTonnageTotalB2b = 0.000m;
        public decimal? ObligatedTonnageTotalB2c = 0.000m;

        public List<AatfObligatedData> AatfObligatedData;

        public ReturnViewModel Map(ReturnData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            if (source.NonObligatedData != null)
            {
                foreach (var category in source.NonObligatedData)
                {
                    if (category.Dcf && category.Tonnage != null)
                    {
                        NonObligatedTonnageTotalDcf += category.Tonnage;
                    }
                    else if (!category.Dcf && category.Tonnage != null)
                    {
                        NonObligatedTonnageTotal += category.Tonnage;
                    }
                }
            }

            if (source.ObligatedData != null)
            {
                //var processes = new ObligatedProcess();
                //foreach (var aatf in source.ObligatedData)
                //{
                //    var aatfWeeData = new AatfObligatedData(aatf.AatfId, aatf.AatfName, new List<ProcessObligatedData>());
                //    foreach (var process in aatf.ProcessObligatedData)
                //    {
                //        foreach (var dataEntry in process.WeeeData)
                //        {
                //            ObligatedTonnageTotalB2b += dataEntry.B2B;
                //            ObligatedTonnageTotalB2c += dataEntry.B2C;
                //        }
                //
                //        var weeeDataTotal = new ObligatedData(0, ObligatedTonnageTotalB2b, ObligatedTonnageTotalB2c);
                //        var processName = processes.ObligatedProcessList[process.ProcessCategoryId];
                //        var processWeeeDataTotal = new ProcessObligatedData(process.ProcessCategoryId, processName, new List<ObligatedData>() { weeeDataTotal }, new List<ObligatedCategoryValue>());
                //
                //        aatfWeeData.ProcessObligatedData.Add(processWeeeDataTotal);
                //    }
                //
                //    AatfObligatedData.Add(aatfWeeData);
                //}
            }

            return new ReturnViewModel(source.Quarter, source.QuarterWindow, source.Quarter.Year, CheckIfTonnageIsNull(NonObligatedTonnageTotal), CheckIfTonnageIsNull(NonObligatedTonnageTotalDcf), AatfObligatedData, source.ReturnOperatorData);
        }

        public string CheckIfTonnageIsNull(decimal? tonnage)
        {
            return (tonnage != null) ? tonnage.ToTonnageDisplay() : "0.000";
        }
    }
}