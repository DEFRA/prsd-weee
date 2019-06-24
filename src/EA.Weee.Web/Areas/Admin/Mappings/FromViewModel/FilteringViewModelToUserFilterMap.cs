namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Weee.Core.User;
    using EA.Weee.Web.Areas.Admin.ViewModels.User;
    using Prsd.Core;
    using Prsd.Core.Mapper;

    public class FilteringViewModelToUserFilterMap : IMap<FilteringViewModel, UserFilter>
    {
        public FilteringViewModelToUserFilterMap()
        {
        }

        public UserFilter Map(FilteringViewModel source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new UserFilter
            {
                OrganisationName = source.OrganisationName,
                Status = source.Status,
                Name = source.Name
            };
        }
    }
}