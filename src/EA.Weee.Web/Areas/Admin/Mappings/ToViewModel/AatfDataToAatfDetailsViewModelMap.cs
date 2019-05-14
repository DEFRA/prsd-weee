namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;

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
                ContactData = source.ContactData,
                CanEditContactDetails = source.ContactData.CanEditContactDetails
            };

            if (source.AatfData.ApprovalDate != default(DateTime))
            {
                viewModel.ApprovalDate = source.AatfData.ApprovalDate.GetValueOrDefault();
            }

            return viewModel;
        }
    }
}