namespace EA.Prsd.Core.Mapper
{
    public interface IMapWithParameter<in TSource, in TParameter, out TTarget>
    {
        TTarget Map(TSource source, TParameter parameter);
    }
}