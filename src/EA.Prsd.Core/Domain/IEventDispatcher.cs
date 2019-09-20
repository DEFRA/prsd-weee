namespace EA.Prsd.Core.Domain
{
    using System.Threading.Tasks;

    public interface IEventDispatcher
    {
        Task Dispatch(IEvent @event);
    }
}