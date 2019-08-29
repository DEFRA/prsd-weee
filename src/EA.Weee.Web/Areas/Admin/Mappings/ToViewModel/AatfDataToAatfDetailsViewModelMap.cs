namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin.AatfReports;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.ViewModels.Shared.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AatfDataToAatfDetailsViewModelMap : IMap<AatfDataToAatfDetailsViewModelMapTransfer, AatfDetailsViewModel>
    {
        private readonly IAddressUtilities addressUtilities;
        private readonly IMap<AatfSubmissionHistoryData, AatfSubmissionHistoryViewModel> aatfSubmissionHistoryMap;

        public AatfDataToAatfDetailsViewModelMap(IAddressUtilities addressUtilities,
            IMap<AatfSubmissionHistoryData, AatfSubmissionHistoryViewModel> aatfSubmissionHistoryMap)
        {
            this.addressUtilities = addressUtilities;
            this.aatfSubmissionHistoryMap = aatfSubmissionHistoryMap;
        }

        public AatfDetailsViewModel Map(AatfDataToAatfDetailsViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.AatfData, source.AatfData);

            AatfDetailsViewModel viewModel = new AatfDetailsViewModel()
            {
                Id = source.AatfData.Id,
                Name = source.AatfData.Name,
                ApprovalNumber = source.AatfData.ApprovalNumber,
                CompetentAuthority = source.AatfData.CompetentAuthority,
                AatfStatus = source.AatfData.AatfStatus,
                SiteAddress = source.AatfData.SiteAddress,
                Size = source.AatfData.Size,
                ContactData = source.AatfData.Contact,
                CanEdit = source.AatfData.Contact.CanEditContactDetails,
                Organisation = source.AatfData.Organisation,
                OrganisationAddress = source.OrganisationString,
                FacilityType = source.AatfData.FacilityType,
                ComplianceYear = source.AatfData.ComplianceYear,
                SiteAddressLong = addressUtilities.FormattedAddress(source.AatfData.SiteAddress, false),
                ContactAddressLong = addressUtilities.FormattedAddress(source.AatfData.Contact.AddressData, false),
                PanArea = source.AatfData.PanAreaData,
                LocalArea = source.AatfData.LocalAreaData,
                ComplianceYearList = source.ComplianceYearList,
                AatfId = source.AatfData.AatfId,
                SelectedComplianceYear = source.AatfData.ComplianceYear,
                CurrentDate = source.CurrentDate
            };

            if (source.AssociatedAatfs != null)
            {
                var filteredList = RemoveOlderAatfs(source.AssociatedAatfs);
                var associatedAEs = filteredList.Where(a => a.FacilityType == FacilityType.Ae && a.Id != source.AatfData.Id && a.AatfId != source.AatfData.AatfId).ToList();
                source.AssociatedAatfs = filteredList.Where(a => a.Id != source.AatfData.Id && a.FacilityType == FacilityType.Aatf && a.AatfId != source.AatfData.AatfId).ToList();
                viewModel.AssociatedAatfs = source.AssociatedAatfs;
                viewModel.AssociatedAes = associatedAEs;
            }

            if (source.AssociatedSchemes != null)
            {
                viewModel.AssociatedSchemes = source.AssociatedSchemes;
            }

            if (source.AatfData.ApprovalDate != default(DateTime))
            {
                viewModel.ApprovalDate = source.AatfData.ApprovalDate.GetValueOrDefault();
            }

            if (source.SubmissionHistory != null && source.SubmissionHistory.Any())
            {
                viewModel.SubmissionHistoryData = source.SubmissionHistory.Select(s => aatfSubmissionHistoryMap.Map(s)).ToList();
            }

            return viewModel;
        }

        private List<AatfDataList> RemoveOlderAatfs(List<AatfDataList> source)
        {
            var listToBeReturned = new List<AatfDataList>();

            foreach (var aatf in source)
            {
                if (source.Any(m => m.AatfId == aatf.AatfId && m.Id != aatf.Id))
                {
                    var latestAatf = source.Where(m => m.AatfId == aatf.AatfId).OrderByDescending(m => m.ApprovalDate).First();

                    if (!listToBeReturned.Contains(latestAatf))
                    {
                        listToBeReturned.Add(latestAatf);
                    }
                }
                else
                {
                    listToBeReturned.Add(aatf);
                }
            }
            return listToBeReturned;
        }
    }
}