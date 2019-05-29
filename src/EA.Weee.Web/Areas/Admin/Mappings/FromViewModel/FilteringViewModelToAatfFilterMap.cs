namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class FilteringViewModelToAatfFilterMap : IMap<FilteringViewModel, AatfFilter>
    {
        public FilteringViewModelToAatfFilterMap()
        {
        }

        public AatfFilter Map(FilteringViewModel source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new AatfFilter
            {
                ApprovalNumber = source.ApprovalNumber,
                Name = source.Name
            };
        }
    }
}
