namespace EA.Prsd.Core.Mapper
{
    using System;

    public interface ITypeResolver
    {
        Type GetType(object source);
    }
}