namespace EA.Prsd.Core.DataAccess.Mapper
{
    using EA.Prsd.Core.Mapper;
    using System;
    using System.Data.Entity.Core.Objects;

    public class EfTypeResolver : ITypeResolver
    {
        public Type GetType(object source)
        {
            return ObjectContext.GetObjectType(source.GetType());
        }
    }
}
