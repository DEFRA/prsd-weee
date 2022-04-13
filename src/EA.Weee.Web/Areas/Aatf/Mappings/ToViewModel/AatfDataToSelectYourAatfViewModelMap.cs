namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.Mappings.Filters;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class AatfDataToSelectYourAatfViewModelMap : IMap<AatfDataToSelectYourAatfViewModelMapTransfer, SelectYourAatfViewModel>
    {
        private readonly IAatfDataFilter<List<AatfData>, FacilityType> filter;

        public AatfDataToSelectYourAatfViewModelMap(IAatfDataFilter<List<AatfData>, FacilityType> filter)
        {
            this.filter = filter;
        }

        public SelectYourAatfViewModel Map(AatfDataToSelectYourAatfViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new SelectYourAatfViewModel
            {
                OrganisationId = source.OrganisationId,
                AatfList = filter.Filter(source.AatfList, source.FacilityType, false).OrderBy(o => o.Name).ToList(),
            };

            return model;
        }
    }
}