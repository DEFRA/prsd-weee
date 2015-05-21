namespace EA.Prsd.Core.DataAccess.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Newtonsoft.Json;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Prsd.Core.Domain.Auditing;
    using Serialization;

    public static class AuditorExtensions
    {
        private static readonly ConcurrentDictionary<string, string> TableNameCache =
            new ConcurrentDictionary<string, string>();

        public static void AuditChanges(this DbContext context, Guid userId)
        {
            context.Set<AuditLog>().AddRange(CreateAuditLog(context, userId));
        }

        private static IEnumerable<AuditLog> CreateAuditLog(DbContext context, Guid userId)
        {
            var states = new[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };

            foreach (var entity in context.ChangeTracker.Entries<Entity>().Where(p => states.Contains(p.State)))
            {
                var entityType = GetEntityType(entity.Entity.GetType());
                var tableName = GetCachedTableName(entityType, context);
                var entityId = (Guid)typeof(Entity).GetProperty("Id").GetValue(entity.Entity);

                AuditLog auditLog;
                if (entity.State == EntityState.Added)
                {
                    auditLog = new AuditLog(userId, SystemTime.UtcNow, EventType.Added, tableName, entityId, null,
                        SerializeObject(entity.CurrentValues.ToObject()));
                }
                else if (entity.State == EntityState.Deleted)
                {
                    auditLog = new AuditLog(userId, SystemTime.UtcNow, EventType.Deleted, tableName, entityId,
                        SerializeObject(entity.OriginalValues.ToObject()), null);
                }
                else if (entity.State == EntityState.Modified)
                {
                    auditLog = new AuditLog(userId, SystemTime.UtcNow, EventType.Modified, tableName, entityId,
                        SerializeObject(entity.OriginalValues.ToObject()),
                        SerializeObject(entity.CurrentValues.ToObject()));
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unsupported EntityState: {0}", entity.State));
                }

                yield return auditLog;
            }
        }

        private static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value,
                new JsonSerializerSettings { ContractResolver = new IgnoreEntityRelationsContractResolver(true) });
        }

        private static Type GetEntityType(Type entityType)
        {
            if (entityType.Namespace == "System.Data.Entity.DynamicProxies")
            {
                return GetEntityType(entityType.BaseType);
            }

            return entityType;
        }

        private static string GetCachedTableName(Type type, DbContext context)
        {
            var key = type.FullName;
            return GetFromCache(TableNameCache, key, k => GetTableName(type, context));
        }

        private static string GetTableName(Type type, DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                .GetItems<EntityType>(DataSpace.OSpace)
                .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                .Single()
                .EntitySetMappings
                .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            var tableSchema = (string)table.MetadataProperties["Schema"].Value ?? table.Schema;
            var tableName = (string)table.MetadataProperties["Table"].Value ?? table.Name;

            // Return the table name from the storage entity set
            return string.Format("[{0}].[{1}]", tableSchema, tableName);
        }

        private static TVal GetFromCache<TKey, TVal>(ConcurrentDictionary<TKey, TVal> dictionary, TKey key,
            Func<TKey, TVal> valueFactory)
        {
            lock (dictionary)
            {
                return dictionary.GetOrAdd(key, valueFactory);
            }
        }
    }
}