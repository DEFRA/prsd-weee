namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;

    public class ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMap : IMap<ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer, CanNotFoundTreatmentFacilityViewModel>
    {
        public CanNotFoundTreatmentFacilityViewModel Map(ReturnAndAatfToCanNotFoundTreatmentFacilityViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewModel = new CanNotFoundTreatmentFacilityViewModel()
            {
                ReturnId = source.Return.Id,
                AatfId = source.AatfId,
                OrganisationId = source.Return.OrganisationData.Id,
                IsCanNotFindLinkClick = source.IsCanNotFindLinkClick,
                AatfName = source.AatfName
            };

            return viewModel;
        }
    }
}