namespace EA.Prsd.Core.DataAccess.Mapper
{
    using System;
    using System.Data.Entity.Core.Objects;
    using Core.Mapper;

    public class EfTypeResolver : ITypeResolver
    {
        public Type GetType(object source)
        {
            return ObjectContext.GetObjectType(source.GetType());
        }
    }
}
