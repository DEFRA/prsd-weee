namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class ReturnToCheckReturnViewModelMap : IMap<ReturnData, CheckYourReturnViewModel>
    {
        public decimal? TonnageTotal = 0.000m;
        public decimal? TonnageTotalDcf = 0.000m;

        public CheckYourReturnViewModel Map(ReturnData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            foreach (var category in source.NonObligatedData)
            {
                if (category.Dcf && category.Tonnage != null)
                {
                    TonnageTotalDcf += category.Tonnage;
                }
                else if (!category.Dcf && category.Tonnage != null)
                {
                    TonnageTotal += category.Tonnage;
                }
            }

            return new CheckYourReturnViewModel(source.ReturnOperatorData, TonnageTotal, TonnageTotalDcf, source.Quarter, source.QuarterWindow, source.Quarter.Year);
        }
    }
}