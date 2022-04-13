namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using Filters;
    using ViewModels;
    using System.Linq;
    using Core.AatfReturn;

    public class AatfDataToHomeViewModelMap : IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel>
    {
        private readonly IAatfDataFilter<List<AatfData>, FacilityType> filter;

        public AatfDataToHomeViewModelMap(IAatfDataFilter<List<AatfData>, FacilityType> filter)
        {
            this.filter = filter;
        }

        public HomeViewModel Map(AatfDataToHomeViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new HomeViewModel
            {
                FacilityType = source.FacilityType,
                OrganisationId = source.OrganisationId,
                AatfList = filter.Filter(source.AatfList, source.FacilityType, true).OrderBy(o => o.Name).ToList(),
            };

            return model;
        }
    }
}