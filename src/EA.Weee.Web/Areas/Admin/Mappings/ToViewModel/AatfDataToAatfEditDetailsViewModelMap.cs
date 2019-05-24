namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;

    public class AatfDataToAatfEditDetailsViewModelMap : IMap<AatfData, AatfEditDetailsViewModel>
    {
        public AatfDataToAatfEditDetailsViewModelMap()
        {
        }

        public AatfEditDetailsViewModel Map(AatfData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new AatfEditDetailsViewModel()
            {
                Id = source.Id,
                Name = source.Name,
                ApprovalNumber = source.ApprovalNumber,
                CompetentAuthorityId = source.CompetentAuthority.Id,
                AatfStatus = source.AatfStatus.Value,
                AatfStatusList = Enumeration.GetAll<AatfStatus>(),
                SiteAddress = source.SiteAddress,
                Size = source.Size.Value,
                SizeList = Enumeration.GetAll<AatfSize>()
            };

            if (source.ApprovalDate != default(DateTime))
            {
                viewModel.ApprovalDate = source.ApprovalDate.GetValueOrDefault();
            }

            return viewModel;
        }
    }
}