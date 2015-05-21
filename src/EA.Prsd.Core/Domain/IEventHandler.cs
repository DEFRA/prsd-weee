namespace EA.Prsd.Core.Domain
{
    using System.Threading.Tasks;

    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}