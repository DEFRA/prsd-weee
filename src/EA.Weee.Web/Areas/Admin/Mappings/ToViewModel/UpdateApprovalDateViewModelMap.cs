namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels.Aatf;

    public class UpdateApprovalDateViewModelMap : IMap<UpdateApprovalDateViewModelMapTransfer, UpdateApprovalViewModel>
    {
        public UpdateApprovalViewModel Map(UpdateApprovalDateViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            Guard.ArgumentNotNull(() => source.AatfData, source.AatfData);
            Guard.ArgumentNotNull(() => source.Request, source.Request);

            return new UpdateApprovalViewModel()
            {
                AatfId = source.AatfData.Id,
                AatfName = source.AatfData.Name,
                FacilityType = source.AatfData.FacilityType,
                OrganisationId = source.AatfData.Organisation.Id,
                OrganisationName = source.AatfData.Organisation.OrganisationName,
                UpdateApprovalDateData = source.CanApprovalDateBeChangedFlags,
                Request = source.Request
            };
        }
    }
}