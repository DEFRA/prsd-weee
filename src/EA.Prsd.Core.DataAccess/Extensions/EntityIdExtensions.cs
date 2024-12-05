namespace EA.Prsd.Core.DataAccess.Extensions
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Identity;
    using System.Data.Entity;
    using System.Linq;

    public static class EntityIdExtensions
    {
        public static void SetEntityId(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<Entity>()
                .Where(e => e.State == EntityState.Added))
            {
                typeof(Entity).GetProperty("Id").SetValue(entry.Entity, GuidCombGenerator.GenerateComb());
            }
        }
    }
}