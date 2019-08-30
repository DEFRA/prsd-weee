namespace EA.Prsd.Core.Mapper
{
    public interface IMapper
    {
        TResult Map<TSource, TResult>(TSource source);
        TResult Map<TResult>(object source);
        TResult Map<TSource, TParameter, TResult>(TSource source, TParameter parameter);
        TResult Map<TResult>(object source, object parameter);
    }
}