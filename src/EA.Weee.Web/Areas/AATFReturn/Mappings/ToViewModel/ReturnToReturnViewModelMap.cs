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
                    var aatfDataSet = source.ObligatedWeeeReceivedData.Where(s => s.Aatf.Id == aatf.Id).ToList();
                    var obligatedData = new AatfObligatedData(aatf);
                    decimal? obligatedTonnageTotalB2b = null;
                    decimal? obligatedTonnageTotalB2c = null;

                    if (aatfDataSet.Count != 0)
                    {
                        obligatedData.Aatf = (aatfDataSet.FirstOrDefault().Aatf);
                        foreach (var category in aatfDataSet)
                        {
                            if (category.B2B != null)
                            {
                                obligatedTonnageTotalB2b = InitialiseTotalDecimal(obligatedTonnageTotalB2b);
                                obligatedTonnageTotalB2b += category.B2B;
                            }
                            if (category.B2C != null)
                            {
                                obligatedTonnageTotalB2c = InitialiseTotalDecimal(obligatedTonnageTotalB2c);
                                obligatedTonnageTotalB2c += category.B2C;
                            }
                        }
                    }

                    obligatedData.WeeeReceived = new ObligatedCategoryValue(CheckIfTonnageIsNull(obligatedTonnageTotalB2b), CheckIfTonnageIsNull(obligatedTonnageTotalB2c));
                    AatfObligatedData.Add(obligatedData);
                }
            }

            return new ReturnViewModel(source.Quarter, source.QuarterWindow, source.Quarter.Year, CheckIfTonnageIsNull(NonObligatedTonnageTotal), CheckIfTonnageIsNull(NonObligatedTonnageTotalDcf), AatfObligatedData, source.ReturnOperatorData);
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