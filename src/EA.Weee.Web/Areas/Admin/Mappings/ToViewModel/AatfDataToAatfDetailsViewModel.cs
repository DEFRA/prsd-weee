namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;

    public class AatfDataToAatfDetailsViewModel : IMap<AatfData, AatfDetailsViewModel>
    {
        public AatfDetailsViewModel Map(AatfData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            AatfDetailsViewModel viewModel = new AatfDetailsViewModel()
            {
                Id = source.Id,
                Name = source.Name,
                ApprovalNumber = source.ApprovalNumber,
                CompetentAuthority = source.CompetentAuthority,
                AatfStatus = source.AatfStatus,
                SiteAddress = source.SiteAddress,
                Size = source.Size
            };

            if (source.ApprovalDate != default(DateTime))
            {
                viewModel.ApprovalDate = source.ApprovalDate.GetValueOrDefault();
            }

            return viewModel;
        }
    }
}