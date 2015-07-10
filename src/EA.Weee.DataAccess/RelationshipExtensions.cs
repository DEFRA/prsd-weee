namespace EA.Weee.DataAccess
{
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Domain;
    using EA.Prsd.Core.Domain;

    public static class RelationshipExtensions
    {
        // Adapted from http://stackoverflow.com/a/20707599
        public static void DeleteRemovedRelationships(this DbContext context)
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            // Get all deleted child objects which are instances of Entity or derived types.
            var deletedRelationships = objectContext.ObjectStateManager
                .GetObjectStateEntries(EntityState.Deleted)
                .Where(e => e.IsRelationship)
                .Select(e => objectContext.GetObjectByKey((EntityKey)e.OriginalValues[1]))
                .Select(e => new { entity = e, type = ObjectContext.GetObjectType(e.GetType()) })
                .Where(e => typeof(Entity).IsAssignableFrom(e.type));

            foreach (var childObject in deletedRelationships)
            {
                context.Entry(childObject.entity).State = EntityState.Deleted;
            }
        }
    }
}
