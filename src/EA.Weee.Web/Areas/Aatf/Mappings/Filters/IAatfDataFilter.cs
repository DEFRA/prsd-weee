namespace EA.Weee.Web.Areas.Aatf.Mappings.Filters
{
    public interface IAatfDataFilter<T, in TU>
    {
        T Filter(T source, TU filter);
    }
}
