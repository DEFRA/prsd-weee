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

        private TonnageUtilities tonnageUtilities = new TonnageUtilities();
        private AddressUtilities addressUtilities = new AddressUtilities();

        public ReusedOffSiteSummaryListViewModel Map(AddressTonnageSummary source)
        {
            Guard.ArgumentNotNull(() => source, source);
             
            foreach (var address in source.AddressData)
            {
                var addressDataSummary = new AddressDataSummary();
                addressDataSummary.Name = address.Name;
                addressDataSummary.Address = addressUtilities.AddressConcatenate(address);
                AddressDataSummaries.Add(addressDataSummary);
            }

            ViewModel.Addresses = AddressDataSummaries;

            var tonnageTotals = tonnageUtilities.SumObligatedValues(source.ObligatedData);

            ViewModel.B2bTotal = tonnageTotals.B2B;
            ViewModel.B2cTotal = tonnageTotals.B2C;

            return ViewModel;
        }
    }
}