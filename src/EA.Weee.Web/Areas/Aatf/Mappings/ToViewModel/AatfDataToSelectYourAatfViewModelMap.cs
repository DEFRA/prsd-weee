namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Linq;
    using EA.Weee.Web.Areas.Aatf.Mappings.Filters;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class AatfDataToSelectYourAatfViewModelMap : IMap<AatfDataToSelectYourAatfViewModelMapTransfer, SelectYourAatfViewModel>
    {
        public SelectYourAatfViewModel Map(AatfDataToSelectYourAatfViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);
            var aatfDataFilter = new AatfDataFilter();

            var model = new SelectYourAatfViewModel
            {
                OrganisationId = source.OrganisationId,
                AatfList = aatfDataFilter.Filter(source.AatfList, source.FacilityType).OrderBy(o => o.Name).ToList(),
            };

            return model;
        }
    }
}