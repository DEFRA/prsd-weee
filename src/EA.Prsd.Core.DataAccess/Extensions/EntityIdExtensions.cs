namespace EA.Prsd.Core.DataAccess.Extensions
{
    using System.Data.Entity;
    using System.Linq;
    using Prsd.Core.Domain;
    using Prsd.Core.Identity;

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