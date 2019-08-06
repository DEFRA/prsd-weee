namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.CopyAatf;
    public class CopyAatfDataToCopyAatfViewModelMap : IMap<AatfData, CopyAatfViewModel>
    {
        public CopyAatfDataToCopyAatfViewModelMap()
        {
        }

        public CopyAatfViewModel Map(AatfData source)
        {
            Guard.ArgumentNotNull(() => source, source);
            return MappingHelper.MapCopyFacility(new CopyAatfViewModel(), source);
        }
    }
}