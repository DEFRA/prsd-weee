namespace EA.Prsd.Core.DataAccess.Extensions
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;

    public static class DomainEventExtensions
    {
        // Adapted from https://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/
        public static async Task DispatchEvents(this DbContext context, IEventDispatcher dispatcher)
        {
            var entitiesWithEvents = context.ChangeTracker.Entries<Entity>()
                .Select(p => p.Entity)
                .Where(p => p.Events.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.Events.ToArray();
                entity.ClearEvents();
                foreach (var @event in events)
                {
                    await dispatcher.Dispatch(@event);
                }
            }
        }
    }
}