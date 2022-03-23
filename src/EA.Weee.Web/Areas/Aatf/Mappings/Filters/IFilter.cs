namespace EA.Weee.Web.Areas.Aatf.Mappings.Filters
{
    public interface IFilter<T, U>
    {
        T Filter(T source, U filter);
    }
}
