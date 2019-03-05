namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class AddressDataToReusedOffSiteSummaryListViewModelMap : IMap<List<AddressData>, ReusedOffSiteSummaryListViewModel>
    {
        public List<AddressDataSummary> AddressDataSummaries = new List<AddressDataSummary>();

        public ReusedOffSiteSummaryListViewModel Map(List<AddressData> source)
        {
            Guard.ArgumentNotNull(() => source, source);

            foreach (var address in source)
            {
                var addressDataSummary = new AddressDataSummary();
                addressDataSummary.Name = address.Name;
                addressDataSummary.Address = AddressConcatenation(address);
            }

            return new ReusedOffSiteSummaryListViewModel(AddressDataSummaries);
        }

        private string AddressConcatenation(AddressData addressData)
        {
            var address = string.Empty;
            return string.Empty;
        }
    }
}