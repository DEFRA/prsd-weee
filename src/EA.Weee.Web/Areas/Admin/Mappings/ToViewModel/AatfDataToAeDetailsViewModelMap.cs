namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Ae;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AatfDataToAeDetailsViewModelMap : IMap<AatfDataToAeDetailsViewModelMapTransfer, AeDetailsViewModel>
    {
        public AatfDataToAeDetailsViewModelMap()
        {
        }

        public AeDetailsViewModel Map(AatfDataToAeDetailsViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            AeDetailsViewModel viewModel = new AeDetailsViewModel()
            {
                Id = source.AatfData.Id,
                Name = source.AatfData.Name,
                ApprovalNumber = source.AatfData.ApprovalNumber,
                CompetentAuthority = source.AatfData.CompetentAuthority,
                AatfStatus = source.AatfData.AatfStatus,
                SiteAddress = source.AatfData.SiteAddress,
                Size = source.AatfData.Size,
                ContactData = source.AatfData.Contact,
                Organisation = source.AatfData.Organisation,
                OrganisationAddress = source.OrganisationString
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