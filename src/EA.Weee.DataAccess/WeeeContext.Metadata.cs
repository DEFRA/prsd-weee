namespace EA.Weee.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    public partial class WeeeContext
    {
        private readonly Dictionary<Type, EntitySetBase> mappingCache = new Dictionary<Type, EntitySetBase>();

        private string GetTableName(Type type)
        {
            var es = GetEntitySet(type);

            return string.Format("[{0}].[{1}]",
                es.MetadataProperties["Schema"].Value,
                es.MetadataProperties["Table"].Value);
        }

        private string GetPrimaryKeyName(Type type)
        {
            var es = GetEntitySet(type);

            return es.ElementType.KeyMembers[0].Name;
        }

        private EntitySetBase GetEntitySet(Type type)
        {
            if (!mappingCache.ContainsKey(type))
            {
                var octx = ((IObjectContextAdapter)this).ObjectContext;
                var typeName = ObjectContext.GetObjectType(type).Name;

                var es = octx.MetadataWorkspace
                                .GetItemCollection(DataSpace.SSpace)
                                .GetItems<EntityContainer>()
                                .SelectMany(c => c.BaseEntitySets
                                                .Where(e => e.Name == typeName))
                                .FirstOrDefault();

                if (es == null)
                {
                    throw new ArgumentException("Entity type not found in GetTableName", typeName);
                }

                mappingCache.Add(type, es);
            }

            return mappingCache[type];
        }
    }
}
