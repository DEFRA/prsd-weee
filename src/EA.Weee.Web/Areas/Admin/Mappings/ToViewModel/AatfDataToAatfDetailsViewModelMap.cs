namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AatfDataToAatfDetailsViewModelMap : IMap<AatfDataToAatfDetailsViewModelMapTransfer, AatfDetailsViewModel>
    {
        public AatfDataToAatfDetailsViewModelMap()
        {
        }

        public AatfDetailsViewModel Map(AatfDataToAatfDetailsViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

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
                SiteAddressLong = source.SiteAddressString
            };

            if (source.AssociatedAatfs != null)
            {
                List<AatfDataList> associatedAEs = source.AssociatedAatfs.Where(a => a.FacilityType == FacilityType.Ae).ToList();
                source.AssociatedAatfs = source.AssociatedAatfs.Where(a => a.Id != source.AatfData.Id && a.FacilityType == FacilityType.Aatf).ToList();
                viewModel.AssociatedAatfs = source.AssociatedAatfs;
                viewModel.AssociatedAEs = associatedAEs;
            }

            if (source.AssociatedSchemes != null)
            {
                viewModel.AssociatedSchemes = source.AssociatedSchemes;
            }

            if (source.AatfData.ApprovalDate != default(DateTime))
            {
                viewModel.ApprovalDate = source.AatfData.ApprovalDate.GetValueOrDefault();
            }

            return viewModel;
        }
    }
}