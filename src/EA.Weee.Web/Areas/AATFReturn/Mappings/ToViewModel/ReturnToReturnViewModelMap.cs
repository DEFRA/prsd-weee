namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnToReturnViewModelMap : IMap<ReturnData, ReturnViewModel>
    {
        public decimal? NonObligatedTonnageTotal = 0.000m;
        public decimal? NonObligatedTonnageTotalDcf = 0.000m;

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

            return new ReturnViewModel(source.Quarter, source.QuarterWindow, source.Quarter.Year, CheckIfTonnageIsNull(NonObligatedTonnageTotal), CheckIfTonnageIsNull(NonObligatedTonnageTotalDcf));
        }

        public string CheckIfTonnageIsNull(decimal? tonnage)
        {
            return (tonnage != null) ? tonnage.ToTonnageDisplay() : "0.000";
        }
    }
}