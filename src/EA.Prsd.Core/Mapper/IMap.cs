namespace EA.Prsd.Core.Mapper
{
    public interface IMap<in TSource, out TTarget>
    {
        TTarget Map(TSource source);
    }
}
