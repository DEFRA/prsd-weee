namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Aatf.ViewModels;

    public class AatfDataToHomeViewModelMap : IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel>
    {
        public HomeViewModel Model = new HomeViewModel();

        public AatfDataToHomeViewModelMap()
        {
        }

        public HomeViewModel Map(AatfDataToHomeViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            Model.IsAE = source.IsAE;
            Model.OrganisationId = source.OrganisationId;
            Model.AatfList = source.AatfList;

            return Model;
        }
    }
}