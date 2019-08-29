namespace EA.Prsd.Core.Mapper
{
    using System;

    public interface IMapWithParentObjectId<in TSource, out TTarget>
    {
        TTarget Map(TSource source, Guid parentId);
    }
}