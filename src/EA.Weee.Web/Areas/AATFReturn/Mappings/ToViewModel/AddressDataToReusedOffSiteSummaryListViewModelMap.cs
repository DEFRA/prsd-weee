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
        public ReusedOffSiteSummaryListViewModel ViewModel = new ReusedOffSiteSummaryListViewModel();
        public List<AddressDataSummary> AddressDataSummaries = new List<AddressDataSummary>();

        public ReusedOffSiteSummaryListViewModel Map(List<AddressData> source)
        {
            Guard.ArgumentNotNull(() => source, source);
             
            foreach (var address in source)
            {
                var addressDataSummary = new AddressDataSummary();
                addressDataSummary.Name = address.Name;
                addressDataSummary.Address = AddressConcatenate(address);
                AddressDataSummaries.Add(addressDataSummary);
            }

            ViewModel.Addresses = AddressDataSummaries;

            return ViewModel;
        }

        private string AddressConcatenate(AddressData addressData)
        {
            var address = string.Empty;

            address = addressData.Address1;

            address = addressData.Address2 == null ? address : StringConcatenate(address, addressData.Address2);

            address = StringConcatenate(address, addressData.TownOrCity);

            address = addressData.CountyOrRegion == null ? address : StringConcatenate(address, addressData.CountyOrRegion);

            address = addressData.Postcode == null ? address : StringConcatenate(address, addressData.Postcode);

            address = StringConcatenate(address, addressData.CountryName);
            
            return address;
        }

        private string StringConcatenate(string address, string input)
        {
            return $"{address}, {input}";
        }
    }
}