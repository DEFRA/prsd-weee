namespace EA.Prsd.Core.Mapper
{
    using System;

    public class DefaultTypeResolver : ITypeResolver
    {
        public Type GetType(object source)
        {
            return source.GetType();
        }
    }
}