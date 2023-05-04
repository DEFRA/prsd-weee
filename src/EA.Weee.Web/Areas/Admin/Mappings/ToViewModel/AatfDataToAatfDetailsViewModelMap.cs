namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Admin.AatfReports;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;
    using System.Linq;
    using Core.Shared;
    using ViewModels.Shared;

    public class AatfDataToAatfDetailsViewModelMap : IMap<AatfDataToAatfDetailsViewModelMapTransfer, AatfDetailsViewModel>
    {
        private readonly IAddressUtilities addressUtilities;
        private readonly IMap<AatfSubmissionHistoryData, AatfSubmissionHistoryViewModel> aatfSubmissionHistoryMap;
        private readonly IMapper mapper;

        public AatfDataToAatfDetailsViewModelMap(IAddressUtilities addressUtilities,
            IMap<AatfSubmissionHistoryData, AatfSubmissionHistoryViewModel> aatfSubmissionHistoryMap, IMapper mapper)
        {
            this.addressUtilities = addressUtilities;
            this.aatfSubmissionHistoryMap = aatfSubmissionHistoryMap;
            this.mapper = mapper;
        }

        public AatfDetailsViewModel Map(AatfDataToAatfDetailsViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.AatfData, source.AatfData);

            var viewModel = new AatfDetailsViewModel()
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
                OrganisationAddressData = new AatfAddressData()
                {
                    Address1 = source.AatfData.Organisation.BusinessAddress.Address1,
                    Address2 = source.AatfData.Organisation.BusinessAddress.Address2,
                    TownOrCity = source.AatfData.Organisation.BusinessAddress.TownOrCity,
                    Postcode = source.AatfData.Organisation.BusinessAddress.Postcode,
                    CountryName = source.AatfData.Organisation.BusinessAddress.CountryName,
                    CountyOrRegion = source.AatfData.Organisation.BusinessAddress.CountyOrRegion,
                    CountryId = source.AatfData.Organisation.BusinessAddress.CountryId,
                    Id = source.AatfData.Organisation.BusinessAddress.Id
                },
                FacilityType = source.AatfData.FacilityType,
                ComplianceYear = source.AatfData.ComplianceYear,
                SiteAddressLong = addressUtilities.FormattedAddress(source.AatfData.SiteAddress, false),
                ContactAddressLong = addressUtilities.FormattedAddress(source.AatfData.Contact.AddressData, false),
                PanArea = source.AatfData.PanAreaData,
                LocalArea = source.AatfData.LocalAreaData,
                ComplianceYearList = source.ComplianceYearList,
                AatfId = source.AatfData.AatfId,
                SelectedComplianceYear = source.AatfData.ComplianceYear,
                CurrentDate = source.CurrentDate,
                AssociatedEntities = mapper.Map<AssociatedEntitiesViewModel>(new AssociatedEntitiesViewModelTransfer() { AatfId = source.AatfData.AatfId, AssociatedAatfs = source.AssociatedAatfs, AssociatedSchemes = source.AssociatedSchemes })
            };

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
    }
}