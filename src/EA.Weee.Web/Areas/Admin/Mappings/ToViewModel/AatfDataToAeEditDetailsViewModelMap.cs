namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;

    public class AatfDataToAeEditDetailsViewModelMap : IMap<AatfData, AeEditDetailsViewModel>
    {
        public AatfDataToAeEditDetailsViewModelMap()
        {
        }

        public AeEditDetailsViewModel Map(AatfData source)
        {
            Guard.ArgumentNotNull(() => source, source);
            return MappingHelper.MapFacility(new AeEditDetailsViewModel(), source);
        }
    }
}