namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class AddressTonnageSummaryToReusedOffSiteSummaryListViewModelMap : IMap<AddressTonnageSummary, ReusedOffSiteSummaryListViewModel>
    {
        public ReusedOffSiteSummaryListViewModel ViewModel = new ReusedOffSiteSummaryListViewModel();
        public List<AddressDataSummary> AddressDataSummaries = new List<AddressDataSummary>();

        private readonly ITonnageUtilities tonnageUtilities;
        private IAddressUtilities addressUtilities;

        public AddressTonnageSummaryToReusedOffSiteSummaryListViewModelMap(
            ITonnageUtilities tonnageUtilities, IAddressUtilities addressUtilities)
        {
            this.tonnageUtilities = tonnageUtilities;
            this.addressUtilities = addressUtilities;
        }

        public ReusedOffSiteSummaryListViewModel Map(AddressTonnageSummary source)
        {
            Guard.ArgumentNotNull(() => source, source);

            ViewModel.Addresses = source.AddressData;

            var tonnageTotals = tonnageUtilities.SumObligatedValues(source.ObligatedData);

            ViewModel.B2bTotal = tonnageTotals.B2B;
            ViewModel.B2cTotal = tonnageTotals.B2C;

            return ViewModel;
        }
    }
}