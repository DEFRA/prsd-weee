namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnToReturnViewModelMap : IMap<ReturnData, ReturnViewModel>
    {
        public decimal? NonObligatedTonnageTotal = null;
        public decimal? NonObligatedTonnageTotalDcf = null;

        public List<AatfObligatedData> AatfObligatedData = new List<AatfObligatedData>();

        public ReturnViewModel Map(ReturnData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            if (source.NonObligatedData != null)
            {
                foreach (var category in source.NonObligatedData)
                {
                    if (category.Dcf && category.Tonnage != null)
                    {
                        NonObligatedTonnageTotalDcf = InitialiseTotalDecimal(NonObligatedTonnageTotalDcf);
                        NonObligatedTonnageTotalDcf += category.Tonnage;
                    }
                    else if (!category.Dcf && category.Tonnage != null)
                    {
                        NonObligatedTonnageTotal = InitialiseTotalDecimal(NonObligatedTonnageTotal);
                        NonObligatedTonnageTotal += category.Tonnage;
                    }
                }
            }

            if (source.Aatfs != null)
            {
                foreach (var aatf in source.Aatfs)
                {
                    var weeeReceivedData = source.ObligatedWeeeReceivedData.Where(s => s.Aatf.Id == aatf.Id).ToList();
                    var weeeReusedData = source.ObligatedWeeeReusedData.Where(s => s.Aatf.Id == aatf.Id).ToList();
                    var obligatedData = new AatfObligatedData(aatf)
                    {
                        WeeeReceived = SumObligatedValues(weeeReceivedData),
                        WeeeReused = SumObligatedValues(weeeReusedData)
                    };

                    AatfObligatedData.Add(obligatedData);
                }
            }

            return new ReturnViewModel(source.Quarter, source.QuarterWindow, source.Quarter.Year, CheckIfTonnageIsNull(NonObligatedTonnageTotal), CheckIfTonnageIsNull(NonObligatedTonnageTotalDcf), AatfObligatedData, source.ReturnOperatorData);
        }

        private ObligatedCategoryValue SumObligatedValues(List<WeeeObligatedData> dataSet)
        {
            decimal? b2bTotal = null;
            decimal? b2cTotal = null;

            if (dataSet.Count != 0)
            {
                foreach (var category in dataSet)
                {
                    if (category.B2B != null)
                    {
                        b2bTotal = InitialiseTotalDecimal(b2bTotal);
                        b2bTotal += category.B2B;
                    }
                    if (category.B2C != null)
                    {
                        b2cTotal = InitialiseTotalDecimal(b2cTotal);
                        b2cTotal += category.B2C;
                    }
                }
            }

            return new ObligatedCategoryValue(CheckIfTonnageIsNull(b2bTotal), CheckIfTonnageIsNull(b2cTotal));
        }

        private decimal? InitialiseTotalDecimal(decimal? tonnage)
        {
            if (tonnage == null)
            {
                tonnage = 0.000m;
            }

            return tonnage;
        }

        private string CheckIfTonnageIsNull(decimal? tonnage)
        {
            return (tonnage != null) ? tonnage.ToTonnageDisplay() : "-";
        }
    }
}