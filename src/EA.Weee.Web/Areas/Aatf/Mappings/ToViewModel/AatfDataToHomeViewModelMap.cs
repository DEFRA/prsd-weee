namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Aatf.Mappings.Filters;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System.Linq;

    public class AatfDataToHomeViewModelMap : IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel>
    {
        public HomeViewModel Map(AatfDataToHomeViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            var aatfDataFilter = new AatfDataFilter();

            var model = new HomeViewModel
            {
                FacilityType = source.FacilityType,
                OrganisationId = source.OrganisationId,
                AatfList = aatfDataFilter.Filter(source.AatfList, source.FacilityType).OrderBy(o => o.Name).ToList(),
            };

            return model;
        }
    }
}