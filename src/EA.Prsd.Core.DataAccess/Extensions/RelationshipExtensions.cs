namespace EA.Prsd.Core.DataAccess.Extensions
{
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Objects.DataClasses;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Domain;

    public static class RelationshipExtensions
    {
        // Adapted from http://stackoverflow.com/a/8541178
        public static void DeleteRemovedRelationships(this DbContext context)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            var deletedRelationships = objectContext.ObjectStateManager
                .GetObjectStateEntries(EntityState.Deleted)
                .Where(e => e.IsRelationship);

            foreach (var relationEntry in deletedRelationships)
            {
                var entry = GetEntityEntryFromRelation(objectContext, relationEntry, 1);

                // Find representation of the relation 
                var relatedEnd = GetRelatedEnd(entry, relationEntry);

                if (!SkipDeletion(relatedEnd.RelationshipSet.ElementType)
                    && IsDomainEntity(entry)
                    && entry.State != EntityState.Deleted)
                {
                    objectContext.DeleteObject(entry.Entity);
                }
            }
        }

        private static IRelatedEnd GetRelatedEnd(ObjectStateEntry entry, ObjectStateEntry relationEntry)
        {
            return entry.RelationshipManager
                .GetAllRelatedEnds()
                .First(r => r.RelationshipSet == relationEntry.EntitySet);
        }

        private static bool IsDomainEntity(ObjectStateEntry entry)
        {
            return typeof(Entity).IsAssignableFrom(ObjectContext.GetObjectType(entry.Entity.GetType()));
        }

        private static ObjectStateEntry GetEntityEntryFromRelation(ObjectContext context, ObjectStateEntry relationEntry,
            int index)
        {
            var firstKey = (EntityKey)relationEntry.OriginalValues[index];
            var entry = context.ObjectStateManager.GetObjectStateEntry(firstKey);
            if (entry.Entity == null)
            {
                // This hilariously populates the Entity if it was null...
                context.GetObjectByKey(firstKey);
            }
            return entry;
        }

        private static bool SkipDeletion(RelationshipType relationshipType)
        {
            return
                // Many-to-many
                relationshipType.RelationshipEndMembers.All(
                    r => r.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                    ||
                // Many-to-ZeroOrOne 
                relationshipType.RelationshipEndMembers[0].RelationshipMultiplicity == RelationshipMultiplicity.Many
                    && relationshipType.RelationshipEndMembers[1].RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne;
        }
    }
}